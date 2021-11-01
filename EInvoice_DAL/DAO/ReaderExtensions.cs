using System.Data.Common;
using EInvoice.Model;
using System.Collections.Generic;
using System;
using System.Linq;
namespace EInvoice.DAL.DAO
{
    public static class ReaderExtensions
    {
        public static IssuerAPIAccessDetails ReadIssuerAPIAccessDetails(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new IssuerAPIAccessDetails()
            {
                ClientId = reader[propertyColumnMappings["ClientId"]] as string,
                ClientSecret = reader[propertyColumnMappings["ClientSecret"]] as string,
                SecurityToken = reader[propertyColumnMappings["SecurityToken"]] as string
            };
        }
        public static ActivityType ReadActivityType(this DbDataReader reader,Dictionary<string,string> propertyColumnMappings)
        {
            return new ActivityType()
            {
                Code = reader[propertyColumnMappings["Code"]] as string,
                EnglishDescription = reader[propertyColumnMappings["EnglishDescription"]] as string,
                ArabicDescription = reader[propertyColumnMappings["ArabicDescription"]] as string
            };
        }
        public static CountryCode ReadCountryCode(this DbDataReader reader,Dictionary<string,string> propertyColumnMappings)
        {
            return new CountryCode()
            {
                Code = reader[propertyColumnMappings["Code"]] as string,
                EnglishDescription = reader[propertyColumnMappings["EnglishDescription"]] as string,
                ArabicDescription = reader[propertyColumnMappings["ArabicDescription"]] as string
            };
        }
        public static TaxType ReadTaxType(this DbDataReader reader,Dictionary<string,string> propertyColumnMappings)
        {
            return new TaxType()
            {
                Code = reader[propertyColumnMappings["Code"]] as string,
                EnglishDescription = reader[propertyColumnMappings["EnglishDescription"]] as string,
                ArabicDescription = reader[propertyColumnMappings["EnglishDescription"]] as string
            };
        }
        public static TaxSubType ReadTaxSubType(this DbDataReader reader,Dictionary<string,string> propertyColumnMappings)
        {
            return new TaxSubType()
            {
                Code = reader[propertyColumnMappings["Code"]] as string,
                EnglishDescription = reader[propertyColumnMappings["EnglishDescription"]] as string,
                ArabicDescription = reader[propertyColumnMappings["ArabicDescription"]] as string
            };
        }
        public static Document ReadDocument(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            Document doc = new Document();
            doc.DateTimeIssued = Convert.ToDateTime(reader[propertyColumnMappings["Document.DateTimeIssued"]]).ToLocalTime();
            doc.Delivery = reader.ReadDelivery(propertyColumnMappings);
            doc.DocumentType = reader[propertyColumnMappings["Document.DocumentType"]] as string;
            doc.DocumentTypeVersion = reader[propertyColumnMappings["Document.DocumentTypeVersion"]] as string;
            doc.ExtraDiscountAmount = Convert.ToDouble((reader[propertyColumnMappings["Document.ExtraDiscountAmount"]] as decimal?)??0);
            doc.InternalId = reader[propertyColumnMappings["Document.InternalId"]] as string;
            doc.Issuer = reader.ReadIssuer(propertyColumnMappings);
            doc.Payment = reader.ReadPayment(propertyColumnMappings);
            doc.Receiver = reader.ReadReceiver(propertyColumnMappings);
            doc.Id = reader[propertyColumnMappings["Document.Id"]] as int?;
            doc.NetAmount = Convert.ToDouble((reader[propertyColumnMappings["Document.NetAmount"]] as decimal?) ?? 0);
            doc.ProformaInvoiceNumber = reader[propertyColumnMappings["Document.ProformaInvoiceNumber"]] as string;
            doc.PurchaseOrderDescription = reader[propertyColumnMappings["Document.PurchaseOrderDescription"]] as string;
            doc.PurchaseOrderReference = reader[propertyColumnMappings["Document.PurchaseOrderReference"]] as string;
            doc.SalesOrderDescription = reader[propertyColumnMappings["Document.SalesOrderDescription"]] as string;
            doc.SalesOrderReference = reader[propertyColumnMappings["Document.SalesOrderReference"]] as string;
            doc.TaxpayerActivityCode = reader[propertyColumnMappings["Document.TaxpayerActivityCode"]] as string;
            doc.TotalAmount = Convert.ToDouble(reader[propertyColumnMappings["Document.TotalAmount"]]);
            doc.TotalSalesAmount = Convert.ToDouble((reader[propertyColumnMappings["Document.TotalSalesAmount"]] as decimal?)??0);
            doc.TotalDiscountAmount = Convert.ToDouble((reader[propertyColumnMappings["Document.TotalDiscountAmount"]] as decimal?)??0);
            doc.Version = reader[propertyColumnMappings["Document.Version"]] as byte[];
            doc.TotalItemsDiscountAmount = Convert.ToDouble((reader[propertyColumnMappings["Document.TotalItemsDiscountAmount"]] as decimal?)??0);
            IList<TaxTotal> taxTotals = new List<TaxTotal>();
            foreach(InvoiceLine line in doc.InvoiceLines)
            {
                foreach(TaxableItem taxableItem in line.TaxableItems)
                {
                    var found = (from tt in taxTotals where tt.TaxType == taxableItem.TaxType
                                select tt).First();
                    if (found == null)
                        taxTotals.Add(new TaxTotal() { TaxType = taxableItem.TaxType, Amount = taxableItem.Amount });
                    else
                        found.Amount += taxableItem.Amount;
                }
            }
            return doc;
        }
        public static TaxableItem ReadTaxableItem(this DbDataReader reader,Dictionary<string,string> propertyColumnMappings)
        {
            return new TaxableItem()
            {
                Amount = Convert.ToDouble((reader[propertyColumnMappings["TaxableItem.Amount"]] as decimal?)??0),
                Rate = Convert.ToDouble((reader[propertyColumnMappings["TaxableItem.Rate"]] as decimal?)??0),
                SubType = reader[propertyColumnMappings["TaxableItem.SubType"]] as string,
                TaxType = reader[propertyColumnMappings["TaxableItem.TaxType"]] as string,
                Version = reader[propertyColumnMappings["TaxableItem.Version"]] as byte[],
                Id = reader[propertyColumnMappings["TaxableItem.Id"]] as int?,
                InvoiceLineId = reader[propertyColumnMappings["TaxableItem.InvoiceLineId"]] as int?
            };
        }
        public static Value ReadValue(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new Value()
            {
                AmountEGP = Convert.ToDouble((reader[propertyColumnMappings["Value.AmountEGP"]] as decimal?)??0),
                AmountSold = Convert.ToDouble((reader[propertyColumnMappings["Value.AmountSold"]] as decimal?)??0),
                CurrencyExchangeRate = Convert.ToDouble((reader[propertyColumnMappings["Value.CurrencyExchangeRate"]] as decimal?)??0),
                CurrencySold = reader[propertyColumnMappings["Value.CurrencySold"]] as string
            };
        }
        public static Discount ReadDiscount(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new Discount()
            {
                Amount = Convert.ToDouble((reader[propertyColumnMappings["Discount.Amount"]] as decimal?)??0),
                Rate = Convert.ToDouble((reader[propertyColumnMappings["Discount.Rate"]] as decimal?)??0)
            };
        }
        public static InvoiceLine ReadInvoiceLine(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new InvoiceLine()
            {
                Description = reader[propertyColumnMappings["InvoiceLine.Description"]] as string,
                InternalCode = reader[propertyColumnMappings["InvoiceLine.InternalCode"]] as string,
                ItemCode = reader[propertyColumnMappings["InvoiceLine.ItemCode"]] as string,
                ItemType = reader[propertyColumnMappings["InvoiceLien.ItemType"]] as string,
                DocumentId = reader[propertyColumnMappings["InvoiceLine.DocumentId"]] as int?,
                Id = reader[propertyColumnMappings["InvoiceLine.Id"]] as int?,
                NetTotal = Convert.ToDouble((reader[propertyColumnMappings["InvoiceLine.NetTotal"]] as decimal?)??0),
                ItemsDiscount = Convert.ToDouble((reader[propertyColumnMappings["InvoiceLine.ItemsDiscount"]] as decimal?)??0),
                Quantity = Convert.ToDouble((reader[propertyColumnMappings["InvoiceLine.Quantity"]] as decimal?)??0),
                SalesTotal = Convert.ToDouble((reader[propertyColumnMappings["InvoiceLine.SalesTotal"]] as decimal?)??0),
                Total = Convert.ToDouble((reader[propertyColumnMappings["InvoiceLine.Total"]] as decimal?)??0),
                UnitType = reader[propertyColumnMappings["InvoiceLine.UnitType"]] as string,
                ValueDifference = Convert.ToDouble((reader[propertyColumnMappings["InvoiceLine.ValueDifference"]] as decimal?)??0),
                Version = reader[propertyColumnMappings["InvoiceLine.Version"]] as byte[],
                TotalTaxableFees = Convert.ToDouble((reader[propertyColumnMappings["InvoiceLine.TotalTaxableFees"]] as decimal?)??0),
                UnitValue = reader.ReadValue(propertyColumnMappings),
                Discount = reader.ReadDiscount(propertyColumnMappings)
            };
        }
        public static APIEnvironment ReadAPIEnvironment(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            APIEnvironment env = new APIEnvironment()
            {
                Id = reader[propertyColumnMappings["APIEnvironment.Id"]] as int?,
                Name = reader[propertyColumnMappings["APIEnvironment.Name"]] as string,
                BaseUri = new Uri(reader[propertyColumnMappings["APIEnvironment.BaseUri"]] as string),
                LogInUri = new Uri(reader[propertyColumnMappings["APIEnvironment.LogInUri"]] as string),
                Version = reader[propertyColumnMappings["APIEnvironment.Version"]] as byte[]
            };
            return env;
        }
        public static IssuerAddress ReadIssuerAddress(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new IssuerAddress()
            {
                AdditionalInformation = reader[propertyColumnMappings["IssuerAddress.AdditionalInformation"]] as string,
                BranchId = reader[propertyColumnMappings["IssuerAddress.BranchId"]] as string,
                BuildingNumber = reader[propertyColumnMappings["IssuerAddress.BuildingNumber"]] as string,
                Country = reader[propertyColumnMappings["IssuerAddress.Country"]] as string,
                Floor = reader[propertyColumnMappings["IssuerAddress.Floor"]] as string,
                Governate = reader[propertyColumnMappings["IssuerAddress.Governate"]] as string,
                Landmark = reader[propertyColumnMappings["IssuerAddress.Landmark"]] as string,
                PostalCode = reader[propertyColumnMappings["IssuerAddress.PostalCode"]] as string,
                RegionCity = reader[propertyColumnMappings["IssuerAddress.RegionCity"]] as string,
                Room = reader[propertyColumnMappings["IssuerAddress.Room"]] as string,
                Street = reader[propertyColumnMappings["IssuerAddress.Street"]] as string
            };
        }
        public static Receiver ReadReceiver(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new Receiver()
            {
                Address = reader.ReadReceiverAddress(propertyColumnMappings),
                Id = reader[propertyColumnMappings["Receiver.Id"]] as string,
                InternalId = reader[propertyColumnMappings["Receiver.InternalId"]] as int?,
                Name = reader[propertyColumnMappings["Receiver.Name"]] as string,
                Type = (reader[propertyColumnMappings["Receiver.Type"]] as string).ToReceiverType(),
                Version = reader[propertyColumnMappings["Receiver.Version"]] as byte[]
            };
        }
        public static ReceiverAddress ReadReceiverAddress(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new ReceiverAddress()
            {
                AdditionalInformation = reader[propertyColumnMappings["ReceiverAddress.AdditionalInformation"]] as string,
                BuildingNumber = reader[propertyColumnMappings["ReceiverAddress.BuildingNumber"]] as string,
                Country = reader[propertyColumnMappings["ReceiverAddress.Country"]] as string,
                Floor = reader[propertyColumnMappings["ReceiverAddress.Floor"]] as string,
                Governate = reader[propertyColumnMappings["ReceiverAddress.Governate"]] as string,
                Landmark = reader[propertyColumnMappings["ReceiverAddress.Landmark"]] as string,
                PostalCode = reader[propertyColumnMappings["ReceiverAddress.PostalCode"]] as string,
                RegionCity = reader[propertyColumnMappings["ReceiverAddress.RegionCity"]] as string,
                Room = reader[propertyColumnMappings["ReceiverAddress.Room"]] as string,
                Street = reader[propertyColumnMappings["ReceiverAddress.Street"]] as string
            };
        }
        public static Issuer ReadIssuer(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            Issuer issuer = new Issuer()
            {
                Id = reader[propertyColumnMappings["Issuer.Id"]] as string,
                Name = reader[propertyColumnMappings["Issuer.Name"]] as string,
                Type = reader[propertyColumnMappings["Issuer.Type"]] as string,
                Version = reader[propertyColumnMappings["Issuer.Version"]] as byte[],
                Address = reader.ReadIssuerAddress(propertyColumnMappings)
            };
            return issuer;
        }
        public static Delivery ReadDelivery(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new Delivery()
            {
                Approach = reader[propertyColumnMappings["Delivery.Approach"]] as string,
                CountryOfOrigin = reader[propertyColumnMappings["Delivery.CountryOfOrigin"]] as string,
                DateValidity = (reader[propertyColumnMappings["Delivery.DateValidity"]] as DateTime?)?.ToString("yyyy-MM-ddTHH:mm:ssZ")??"",
                ExportPort = reader[propertyColumnMappings["Delivery.ExportPort"]] as string,
                GrossWeight = Convert.ToInt32((reader[propertyColumnMappings["Delivery.GrossWeight"]] as decimal?)??0),
                NetWeight = Convert.ToInt32((reader[propertyColumnMappings["Delivery.NetWeight"]] as decimal?)??0),
                Packaging = reader[propertyColumnMappings["Delivery.Packaging"]] as string,
                Terms = reader[propertyColumnMappings["Delivery.Terms"]] as string
            };
        }
        public static Payment ReadPayment(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            return new Payment()
            {
                BankAccountIBAN = reader[propertyColumnMappings["Payment.BankAccountIBAN"]] as string,
                BankAccountNo = reader[propertyColumnMappings["Payment.BankAccountNo"]] as string,
                BankAddress = reader[propertyColumnMappings["Payment.BankAddress"]] as string,
                BankName = reader[propertyColumnMappings["Payment.BankName"]] as string,
                SwiftCode = reader[propertyColumnMappings["Payment.SwiftCode"]] as string,
                Terms = reader[propertyColumnMappings["Payment.Terms"]] as string
            };
        }
        public static DocumentSubmission ReadDocumentSubmission(this DbDataReader reader, Dictionary<string, string> propertyColumnMappings)
        {
            DocumentSubmission document = new DocumentSubmission()
            {
                APIEnvironment = reader.ReadAPIEnvironment(propertyColumnMappings),
                SubmissionUUID = reader[propertyColumnMappings["DocumentSubmission.SubmissionUUID"]] as string,
                UUID = reader[propertyColumnMappings["DocumentSubmission.UUID"]] as string,
                Status = reader[propertyColumnMappings["DocumentSubmission.Status"]] as string,
                SubmissionDate = Convert.ToDateTime(reader[propertyColumnMappings["DocumentSubmission.SubmissionDate"]]),
                Version = reader[propertyColumnMappings["DocumentSubmission.Version"]] as byte[],
                Document = reader.ReadDocument(propertyColumnMappings)
            };
            return document;
        }
    }
}
