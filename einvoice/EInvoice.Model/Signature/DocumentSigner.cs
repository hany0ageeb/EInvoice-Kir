using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using EInvoice.Model;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EInvoice.Signature
{
    public class DocumentSigner
    {
        public void Sign(IList<Document> documents, string securityToken, string dllPath, string tokenIssuerName)
        {
            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
            using (var lib = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, dllPath, AppType.MultiThreaded))
            {
                ISlot slot = lib.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();
                if (slot == null)
                    throw new Exception("No Slot Present...");
                ITokenInfo tokenInfo = slot.GetTokenInfo();
                using (ISession session = slot.OpenSession(SessionType.ReadOnly))
                {
                    session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(securityToken));
                    List<IObjectAttribute> certificateSearchAttributes = new List<IObjectAttribute>
                    {
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
                    };
                    IObjectHandle certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();
                    if (certificate == null)
                    {
                        throw new Exception("No Device Detected");
                    }
                    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.MaxAllowed);
                    X509Certificate2Collection foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, tokenIssuerName, true);
                    X509Certificate2 certForSigning = foundCerts[0];
                    store.Close();
                    foreach(Document document in documents)
                    {
                        string jsonString = JsonConvert.SerializeObject(document, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });
                        string serializedDocument = Serialize(jsonString);
                        byte[] data = Encoding.UTF8.GetBytes(serializedDocument);
                        ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);
                        SignedCms cms = new SignedCms(content, detached:true);
                        CmsSigner signer = new CmsSigner(certForSigning);
                        EssCertIDv2 bouncyCertificate = new EssCertIDv2(new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")), HashBytes(certForSigning.RawData));
                        SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2(new EssCertIDv2[1] { bouncyCertificate });
                        signer.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1");
                        signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
                        signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));
                        cms.ComputeSignature(signer);
                        byte[] output = cms.Encode();
                        document.Signatures.Clear();
                        document.Signatures.Add(new Model.Signature() { SignatureType = "I", Value = Convert.ToBase64String(output, Base64FormattingOptions.None) });
                    }
                }
            }
        }
        public void Sign(Document document,string securityToken, string dllPath, string tokenIssuerName)
        {
            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
            using (var lib = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, dllPath, AppType.MultiThreaded))
            {
                ISlot slot = lib.GetSlotList(SlotsType.WithTokenPresent).First();
                ITokenInfo tokenInfo = slot.GetTokenInfo();
                using (ISession session = slot.OpenSession(SessionType.ReadOnly))
                {
                    session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(securityToken));
                    List<IObjectAttribute> certificateSearchAttributes = new List<IObjectAttribute>
                    {
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
                    };
                    IObjectHandle certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();
                    if (certificate == null)
                    {
                        throw new Exception("No Device Detected");
                    }
                    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.MaxAllowed);
                    X509Certificate2Collection foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, tokenIssuerName, true);
                    X509Certificate2 certForSigning = foundCerts[0];
                    store.Close();
                    string jsonString = JsonConvert.SerializeObject(document, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });
                    string serializedDocument = Serialize(jsonString);
                    byte[] data = Encoding.UTF8.GetBytes(serializedDocument);
                    ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);
                    SignedCms cms = new SignedCms(content, detached:true);
                    CmsSigner signer = new CmsSigner(certForSigning);
                    EssCertIDv2 bouncyCertificate = new EssCertIDv2(new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")), HashBytes(certForSigning.RawData));
                    SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2(new EssCertIDv2[1] { bouncyCertificate });
                    signer.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1");
                    signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
                    signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));
                    cms.ComputeSignature(signer);
                    byte[] output = cms.Encode();
                    document.Signatures.Clear();
                    document.Signatures.Add(new Model.Signature() { SignatureType = "I", Value = Convert.ToBase64String(output, Base64FormattingOptions.None) });
                }
            }
        }
        public string Serialize(string jsonText)
        {
            JObject request = JsonConvert.DeserializeObject<JObject>(jsonText, new JsonSerializerSettings() 
            {
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Decimal,
                DateParseHandling = DateParseHandling.None,
                DateFormatString = "yyyy-MM-ddTHH:mm:ssZ"
            });
            return SerializeToken(request);
        }
        private string SerializeToken(JToken request)
        {
            string serialized = "";
            if (request.Parent == null)
            {
                SerializeToken(request.First);
            }
            else if (request.Type == JTokenType.Property && ((JProperty)request).Name!= "signatures")
            {
                string name = ((JProperty)request).Name.ToUpper();   
                serialized = serialized + "\"" + name + "\"";
                foreach (JToken property2 in (IEnumerable<JToken>)request)
                {
                    if (property2.Type == JTokenType.Object)
                    {
                        serialized += SerializeToken(property2);
                    }
                    if (property2.Type == JTokenType.Boolean || property2.Type == JTokenType.Integer || property2.Type == JTokenType.Float || property2.Type == JTokenType.String || property2.Type == JTokenType.Date)
                    {
                        serialized = serialized + "\"" + property2.Value<string>() + "\"";
                    }
                    if (property2.Type != JTokenType.Array)
                    {
                        continue;
                    }
                    foreach (JToken item in property2.Children())
                    {
                        serialized = serialized + "\"" + ((JProperty)request).Name.ToUpper() + "\"";
                        serialized += SerializeToken(item);
                    }
                }
            }
            if (request.Type == JTokenType.Object)
            {
                foreach (JToken property in request.Children())
                {
                    if (property.Type == JTokenType.Object || property.Type == JTokenType.Property)
                    {
                        serialized += SerializeToken(property);
                    }
                }
            }
            return serialized;
        }
        public string Serialize(Document document)
        {
            StringBuilder sb = new StringBuilder();
            //Document.Issuer
            sb.Append($"\"{"Issuer".ToUpperInvariant()}\"");
            //Document.Issuer.Address
            sb.Append($"\"{"Address".ToUpperInvariant()}\"");
            sb.Append($"\"{"BranchId".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.BranchId}\"");
            sb.Append($"\"{"Country".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.Country}\"");
            sb.Append($"\"{"Governate".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.Governate}\"");
            sb.Append($"\"{"RegionCity".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.RegionCity}\"");
            sb.Append($"\"{"Street".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.Street}\"");
            sb.Append($"\"{"BuildingNumber".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.BuildingNumber}\"");
            sb.Append($"\"{"PostalCode".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.PostalCode}\"");
            sb.Append($"\"{"Floor".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.Floor}\"");
            sb.Append($"\"{"Room".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.Room}\"");
            sb.Append($"\"{"Landmark".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.Landmark}\"");
            sb.Append($"\"{"AdditionalInformation".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Address.AdditionalInformation}\"");
            //Issuer
            sb.Append($"\"{"Type".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Type}\"");
            sb.Append($"\"{"Id".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Id}\"");
            sb.Append($"\"{"Name".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Issuer.Name}\"");
            //Document.Receiver
            sb.Append($"\"{"Receiver".ToUpperInvariant()}\"");
            //Document.Receiver.Address
            sb.Append($"\"{"Address".ToUpperInvariant()}\"");
            sb.Append($"\"{"Country".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.Country}\"");
            sb.Append($"\"{"Governate".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.Governate}\"");
            sb.Append($"\"{"RegionCity".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.RegionCity}\"");
            sb.Append($"\"{"Street".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.Street}\"");
            sb.Append($"\"{"BuildingNumber".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.BuildingNumber}\"");
            sb.Append($"\"{"PostalCode".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.PostalCode}\"");
            sb.Append($"\"{"Floor".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.Floor}\"");
            sb.Append($"\"{"Room".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.Room}\"");
            sb.Append($"\"{"Landmark".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.Landmark}\"");
            sb.Append($"\"{"AdditionalInformation".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Address.AdditionalInformation}\"");
            //Receiver
            sb.Append($"\"{"Type".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Type.ToString()}\"");
            sb.Append($"\"{"Id".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Id}\"");
            sb.Append($"\"{"Name".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Receiver.Name}\"");
            //Document
            sb.Append($"\"{"DocumentType".ToUpperInvariant()}\"");
            sb.Append($"\"{document.DocumentType}\"");
            sb.Append($"\"{"DocumentTypeVersion".ToUpperInvariant()}\"");
            sb.Append($"\"{document.DocumentTypeVersion}\"");
            sb.Append($"\"{"DateTimeIssued".ToUpperInvariant()}\"");
            sb.Append($"\"{document.DateTimeIssued.ToString("yyyy-MM-ddTHH:mm:ssZ")}\"");
            sb.Append($"\"{"TaxpayerActivityCode".ToUpperInvariant()}\"");
            sb.Append($"\"{document.TaxpayerActivityCode}\"");
            sb.Append($"\"{"InternalId".ToUpperInvariant()}\"");
            sb.Append($"\"{document.InternalId}\"");
            sb.Append($"\"{"PurchaseOrderReference".ToUpperInvariant()}\"");
            sb.Append($"\"{document.PurchaseOrderReference}\"");
            sb.Append($"\"{"PurchaseOrderDescription".ToUpperInvariant()}\"");
            sb.Append($"\"{document.PurchaseOrderDescription}\"");
            sb.Append($"\"{"SalesOrderReference".ToUpperInvariant()}\"");
            sb.Append($"\"{document.SalesOrderReference}\"");
            sb.Append($"\"{"SalesOrderDescription".ToUpperInvariant()}\"");
            sb.Append($"\"{document.SalesOrderDescription}\"");
            sb.Append($"\"{"ProformaInvoiceNumber".ToUpperInvariant()}\"");
            sb.Append($"\"{document.ProformaInvoiceNumber}\"");
            //Document.payement
            sb.Append($"\"{"Payment".ToUpperInvariant()}\"");
            sb.Append($"\"{"BankName".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Payment.BankName}\"");
            sb.Append($"\"{"BankAddress".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Payment.BankAddress}\"");
            sb.Append($"\"{"BankAccountNo".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Payment.BankAccountNo}\"");
            sb.Append($"\"{"BankAccountIBAN".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Payment.BankAccountIBAN}\"");
            sb.Append($"\"{"SwiftCode".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Payment.SwiftCode}\"");
            sb.Append($"\"{"Terms".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Payment.Terms}\"");
            //Document.Delivery
            sb.Append($"\"{"Delivery".ToUpperInvariant()}\"");
            sb.Append($"\"{"Approach".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.Approach}\"");
            sb.Append($"\"{"Packaging".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.Packaging}\"");
            sb.Append($"\"{"DateValidity".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.DateValidity??""}\"");
            sb.Append($"\"{"ExportPort".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.ExportPort}\"");
            sb.Append($"\"{"CountryOfOrigin".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.CountryOfOrigin}\"");
            sb.Append($"\"{"GrossWeight".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.GrossWeight?.ToString("0.0####")}\"");
            sb.Append($"\"{"NetWeight".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.NetWeight?.ToString("0.0####")}\"");
            sb.Append($"\"{"Terms".ToUpperInvariant()}\"");
            sb.Append($"\"{document.Delivery.Terms}\"");
            //document.invoiceLines
            sb.Append($"\"{"InvoiceLines".ToUpperInvariant()}\"");
            foreach (InvoiceLine line in document.InvoiceLines)
            {
                sb.Append($"\"{"InvoiceLines".ToUpperInvariant()}\"");
                sb.Append($"\"{"Description".ToUpperInvariant()}\"");
                sb.Append($"\"{line.Description}\"");
                sb.Append($"\"{"ItemType".ToUpperInvariant()}\"");
                sb.Append($"\"{line.ItemType}\"");
                sb.Append($"\"{"ItemCode".ToUpperInvariant()}\"");
                sb.Append($"\"{line.ItemCode}\"");
                sb.Append($"\"{"InternalCode".ToUpperInvariant()}\"");
                sb.Append($"\"{line.InternalCode}\"");
                sb.Append($"\"{"UnitType".ToUpperInvariant()}\"");
                sb.Append($"\"{line.UnitType}\"");
                sb.Append($"\"{"Quantity".ToUpperInvariant()}\"");
                sb.Append($"\"{line.Quantity.ToString("0.0####", CultureInfo.InvariantCulture)}\"");
                sb.Append($"\"{"UnitValue".ToUpperInvariant()}\"");
                sb.Append($"\"{"CurrencySold".ToUpperInvariant()}\"");
                sb.Append($"\"{line.UnitValue.CurrencySold}\"");
                sb.Append($"\"{"AmountSold".ToUpperInvariant()}\"");
                sb.Append($"\"{line.UnitValue.AmountSold?.ToString("0.0####")}\"");
                sb.Append($"\"{"CurrencyExchangeRate".ToUpperInvariant()}\"");
                sb.Append($"\"{line.UnitValue.CurrencyExchangeRate?.ToString("0.0####")}\"");
                sb.Append($"\"{"AmountEGP".ToUpperInvariant()}\"");
                sb.Append($"\"{line.UnitValue.AmountEGP.ToString("0.0####")}\"");
                sb.Append($"\"{"SalesTotal".ToUpperInvariant()}\"");
                sb.Append($"\"{line.SalesTotal.ToString("0.0####")}\"");
                sb.Append($"\"{"ValueDifference".ToUpperInvariant()}\"");
                sb.Append($"\"{line.ValueDifference.ToString("0.0####")}\"");
                sb.Append($"\"{"TotalTaxableFees".ToUpperInvariant()}\"");
                sb.Append($"\"{line.TotalTaxableFees.ToString("0.0####")}\"");
                sb.Append($"\"{"Discount".ToUpperInvariant()}\"");
                sb.Append($"\"{"Rate".ToUpperInvariant()}\"");
                sb.Append($"\"{line.Discount.Rate?.ToString("0.0####")}\"");
                sb.Append($"\"{"Amount".ToUpperInvariant()}\"");
                sb.Append($"\"{line.Discount.Amount?.ToString("0.0####")}\"");
                sb.Append($"\"{"NetTotal".ToUpperInvariant()}\"");
                sb.Append($"\"{line.NetTotal.ToString("0.0####")}\"");
                sb.Append($"\"{"ItemsDiscount".ToUpperInvariant()}\"");
                sb.Append($"\"{line.ItemsDiscount.ToString("0.0####")}\"");
                sb.Append($"\"{"TaxableItems".ToUpperInvariant()}\"");
                foreach (TaxableItem taxableItem in line.TaxableItems)
                {
                    sb.Append($"\"{"TaxableItems".ToUpperInvariant()}\"");
                    sb.Append($"\"{"TaxType".ToUpperInvariant()}\"");
                    sb.Append($"\"{taxableItem.TaxType}\"");
                    sb.Append($"\"{"SubType".ToUpperInvariant()}\"");
                    sb.Append($"\"{taxableItem.SubType}\"");
                    sb.Append($"\"{"Rate".ToUpperInvariant()}\"");
                    sb.Append($"\"{taxableItem.Rate.ToString("0.0####")}\"");
                    sb.Append($"\"{"Amount".ToUpperInvariant()}\"");
                    sb.Append($"\"{taxableItem.Amount.ToString("0.0####")}\"");
                }
                sb.Append($"\"{"Total".ToUpperInvariant()}\"");
                sb.Append($"\"{line.Total.ToString("0.0####")}\"");
            }
            //Document
            sb.Append($"\"{"TotalSalesAmount".ToUpperInvariant()}\"");
            sb.Append($"\"{document.TotalSalesAmount.ToString("0.0####")}\"");
            sb.Append($"\"{"TotalDiscountAmount".ToUpperInvariant()}\"");
            sb.Append($"\"{document.TotalDiscountAmount.ToString("0.0####")}\"");
            sb.Append($"\"{"NetAmount".ToUpperInvariant()}\"");
            sb.Append($"\"{document.NetAmount.ToString("0.0####")}\"");
            //document.totaltax
            sb.Append($"\"{"TaxTotals".ToUpperInvariant()}\"");
            foreach(TaxTotal taxTotal in document.TaxTotals)
            {
                sb.Append($"\"{"TaxTotals".ToUpperInvariant()}\"");
                sb.Append($"\"{"TaxType".ToUpperInvariant()}\"");
                sb.Append($"\"{taxTotal.TaxType}\"");
                sb.Append($"\"{"Amount".ToUpperInvariant()}\"");
                sb.Append($"\"{taxTotal.Amount.ToString("0.0####")}\"");
            }
            sb.Append($"\"{"ExtraDiscountAmount".ToUpperInvariant()}\"");
            sb.Append($"\"{document.ExtraDiscountAmount.ToString("0.0####")}\"");
            sb.Append($"\"{"TotalItemsDiscountAmount".ToUpperInvariant()}\"");
            sb.Append($"\"{document.TotalItemsDiscountAmount.ToString("0.0####")}\"");
            sb.Append($"\"{"TotalAmount".ToUpperInvariant()}\"");
            sb.Append($"\"{document.TotalAmount.ToString("0.0####")}\"");
            return sb.ToString();
        }
        public byte[] HashBytes(byte[] input)
        {
            byte[] hashed;
            using (SHA256 sha = SHA256.Create())
            {
                hashed = sha.ComputeHash(input);
            }
            return hashed;
        }
    }
}
