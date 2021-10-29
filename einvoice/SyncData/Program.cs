using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Signature;
using EInvoice.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using EInvoice.DAL.DAO;
using EInvoice.DAL.EInvoiceAPI;
using EInvoice.DAL.APIErrors;
using EInvoice.DesktopUI.Controllers;
using System.IO.Compression;
using System.IO;
using EInvoice.DesktopUI.ViewModel;
using Org.BouncyCastle.Asn1.Ess;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1;

namespace SyncData
{
    class Program
    {
        public static void TestSubmit()
        {
            IDocumentDao documentDao = ObjectFactory.DocumentDao;
            Issuer issuer = ObjectFactory.IssuerDao.Find("205018637");
            APIEnvironment env = ObjectFactory.APIEnvironmentDao.Find().Where(e => e.Id == 1).FirstOrDefault();
            var details = ObjectFactory.IssuerAPIAccessDetailsDao.Find(env, issuer);
            Document doc = documentDao.FindByInternalId("307881").FirstOrDefault();
            DocumentSigner signer = new DocumentSigner();
            string serialized = signer.Serialize(doc);
            signer.Sign(doc, details.SecurityToken,AppSettingsController.Settings.DLLPath,AppSettingsController.Settings.TokenIssuerName);
            EInvoiceAPIRestSharpProxy proxy = new EInvoiceAPIRestSharpProxy(env, details.ClientId, details.ClientSecret);
            var rbody = JsonConvert.SerializeObject(new { documents = new List<Document>() { doc} },Formatting.Indented,new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });
            var result = proxy.SubmitDocuments(rbody);
            Console.WriteLine(result.submissionId);
            Console.ReadLine();
        } 
        public static void CompareTwoSerializationMethods()
        {
            using(DbConnection connection = ObjectFactory.Connection)
            {
                ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
                IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection,taxableItemDao);
                IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
                IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
                IList<Document> docs = documentDao.FindByInternalId("307881");
                DocumentSigner documentSigner = new DocumentSigner();
                string serializa1 = documentSigner.Serialize(docs[0]);
                string serialize2 = documentSigner.Serialize(docs[0]);
                Console.WriteLine(serializa1.Equals(serialize2));
                Console.WriteLine(serializa1.Length==serialize2.Length);
                Console.WriteLine(serializa1.Length);
                Console.WriteLine(serialize2.Length);
                for(int i = 0; i < serializa1.Length; i++)
                {
                    if (serializa1[i] != serialize2[i])
                    {
                        Console.WriteLine($"{serializa1[i]}  --  {serialize2[i]} {serializa1[i] == serialize2[i]}");
                        Console.WriteLine(i);
                    }
                }
                File.WriteAllText(@"d:\myMethod.json", serializa1);
                File.WriteAllText(@"d:\hismethod.json", serialize2);
                Console.ReadLine();
            }
        }
        public static void Sync_Old_DataBase_Engineering_With_Portal_Production(string datasource)
        {
            SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
            stringBuilder.DataSource = datasource;
            stringBuilder.InitialCatalog = "E-Invoice";
            stringBuilder.IntegratedSecurity = false;
            stringBuilder.UserID = "einvoice";
            stringBuilder.Password = "emeit";
            using(SqlConnection connection = new SqlConnection(stringBuilder.ToString()))
            {
                APIEnvironment env = new APIEnvironment()
                {
                    BaseUri = new Uri("https://api.invoicing.eta.gov.eg"),
                    LogInUri = new Uri("https://id.eta.gov.eg/connect/token")
                };
                SqlClientFactory sqlClientFactory = SqlClientFactory.Instance;
                var adapter = new SqlDataAdapter("select * from dbo.EINVOICE_TAXHEADER where PORTALNUMBER = ''", connection);
                SqlCommand updateCommand = new SqlCommand("update dbo.EINVOICE_TAXHEADER set PORTALNUMBER = @uuid,PORTALBATCHNUMBER = @submissionuuid,STATUSONPORTAL=@status");
                updateCommand.Parameters.Add(new SqlParameter("@uuid", SqlDbType.NVarChar, 200, "PORTALNUMBER"));
                updateCommand.Parameters.Add(new SqlParameter("@submissionuuid", SqlDbType.NVarChar, 200, "PORTALBATCHNUMBER"));
                updateCommand.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 255, "STATUSONPORTAL"));
                adapter.UpdateCommand = updateCommand;
                int currentPage = 1;
                DataTable table = new DataTable();
                adapter.Fill(table);
                int rowCounter = 0;
                if (table.Rows.Count > 0)
                {
                    EInvoiceAPIRestSharpProxy proxy = new EInvoiceAPIRestSharpProxy(env, "a7e93449-48d9-48f1-868d-2305bd0938b6", "5db567f7-1ca2-48f5-9967-17b328669b69");
                    var recentDocuments = proxy.GetRecentDocuments(1, 100);
                    do
                    {
                        foreach (var itm in recentDocuments.result)
                        {
                            DataRow row = table.AsEnumerable().FirstOrDefault(rw => { return rw.Field<string>("DOC_INTERNALID") == itm.internalId; });
                            if (row != null)
                            {
                                row.SetField<string>("PORTALNUMBER", itm.uuid);
                                row.SetField<string>("PORTALBATCHNUMBER", itm.uuid);
                                row.SetField<string>("STATUSONPORTAL", itm.status);
                                rowCounter++;
                            }
                        }
                        currentPage++;
                        recentDocuments = proxy.GetRecentDocuments(currentPage, 100);
                    } while (currentPage < recentDocuments.metadata.totalPages && rowCounter <= table.Rows.Count);
                }
                updateCommand.Connection = connection;
                adapter.Update(table);
            }
        }
        public static void PutPackageRequests(IList<string> issuerIds)
        {
            var connection = ObjectFactory.Connection;
            if (connection.State != ConnectionState.Open)
                connection.Open();
                IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
                IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
                IIssuerAPIAccessDetailsDao detailsDao = new IssuerAPIAccessDetailsDaoAdoImpl(connection);
                ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
                IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
                IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
                IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
                IList<Issuer> issuers = new List<Issuer>();
                foreach(string iss_id in issuerIds)
                {
                    issuers.Add(issuerDao.Find(iss_id));
                }
                StringBuilder sb = new StringBuilder();
                foreach (Issuer issuer in issuers)
                {
                    IList<APIEnvironment> environments = environmentDao.Find();
                    Console.WriteLine($"Current Taxpayer: {issuer.Name}");
                    foreach (APIEnvironment environment in environments)
                    {
                        Console.WriteLine($"Current API: {environment.Name}");
                        var accessDetails = detailsDao.Find(environment, issuer);
                        if (accessDetails == null)
                            break;
                        IEInvoiceAPIProxy proxy = new EInvoiceAPIRestSharpProxy(environment, accessDetails.ClientId, accessDetails.ClientSecret);
                        string rid = proxy.RequestDocumentPackage(new DocumentPackageRequest() 
                        {
                            format = "JSON",
                            type = "Full",
                            queryParameters = new QueryParameters()
                            {
                                dateFrom = DateTime.Parse("2021-04-15T00:00:00Z"),
                                dateTo = DateTime.Parse("2021-10-11T23:59:00Z"),
                                statuses = new List<string>() { "Valid" }
                            }
                        });
                        sb.AppendLine($"{issuer.Id}/{issuer.Name}->{environment.Id}/{environment.Name}->{rid}");
                    }
                }
                File.WriteAllText(@"PackageRequests.txt", sb.ToString());
        }
        public static void TryFakeCert()
        {
            HomeController homeController = new HomeController();
            Settings settings = AppSettingsController.Settings;
            using(DbConnection connection = ObjectFactory.Connection)
            {
                ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
                IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
                IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
                IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
                Document doc = documentDao.FindByInternalId("307881").FirstOrDefault();
                string jsonString = "";//= File.ReadAllText(@"D:\SER\EInvoiceApp\Bin\Debug\NotSigned\729238687_299674_Source.json");
                //Document doc = JsonConvert.DeserializeObject<Document>(jsonString);
                //doc.DateTimeIssued = DateTime.Parse("2021-10-01T00:00:00Z");
                DocumentSigner documentSigner = new DocumentSigner();
                jsonString = JsonConvert.SerializeObject(doc,Formatting.Indented,new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });
                string serializedDocument = documentSigner.Serialize(doc);
                System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
                store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
                var certForSigning = store.Certificates.Find(System.Security.Cryptography.X509Certificates.X509FindType.FindByIssuerName, "localhost",true)[0];
                store.Close();
                byte[] data = Encoding.UTF8.GetBytes(serializedDocument);
                ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);
                SignedCms cms = new SignedCms(content, detached: true);
                CmsSigner signer = new CmsSigner(certForSigning);
                EssCertIDv2 bouncyCertificate = new EssCertIDv2(new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")), documentSigner.HashBytes(certForSigning.RawData));
                SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2(new EssCertIDv2[1] { bouncyCertificate });
                signer.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1");
                signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
                signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));
                cms.ComputeSignature(signer);
                byte[] output = cms.Encode();
                string sigValue = Convert.ToBase64String(output, Base64FormattingOptions.None);
                doc.Signatures.Clear();
                doc.Signatures.Add(new EInvoice.Model.Signature() { SignatureType = "I", Value = sigValue });
                IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
                APIEnvironment environment = environmentDao.Find().Where(e => e.Id == 1).FirstOrDefault();
                IIssuerAPIAccessDetailsDao accessDetailsDao = new IssuerAPIAccessDetailsDaoAdoImpl(connection);
                IssuerDaoAdoImpl issuerDao = new IssuerDaoAdoImpl(connection);
                Issuer issuer = issuerDao.Find("205018637");
                var details = accessDetailsDao.Find(environment, issuer);
                IEInvoiceAPIProxy proxy = new EInvoiceAPIRestSharpProxy(environment, details.ClientId, details.ClientSecret);
                string jsonText = JsonConvert.SerializeObject(new { documents = new List<Document>() { doc } }, Formatting.Indented,new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ"});
                File.WriteAllText(@"C:\Users\Home\Desktop\EInvoice\EInvoice\EInvoice\EInvoice.DesktopUI\bin\Debug\Serialized" + doc.InternalId + ".json", serializedDocument);
                File.WriteAllText(@"C:\Users\Home\Desktop\EInvoice\EInvoice\EInvoice\EInvoice.DesktopUI\bin\Debug\Invoices\" + doc.InternalId + ".json", jsonText);
                var result = proxy.SubmitDocuments(jsonText);
                foreach(var accept in result.acceptedDocuments)
                {
                    Console.WriteLine($"{accept.uuid}   {result.submissionId}  {accept.internalId}");
                }
                if (result.acceptedDocuments.Count > 0) 
                {
                    System.Threading.Thread.Sleep(5000);
                    var docEx = proxy.GetDocument(result.acceptedDocuments[0].uuid);
                    Console.WriteLine(docEx.status);
                    Console.WriteLine(docEx.validationResults.status);
                    foreach (var itm in docEx.validationResults.validationSteps)
                    {
                        
                        Console.WriteLine($"{itm.status}-------{itm.error?.Message} ----- {itm.name}");
                    }
                }
                store.Close();
            }
        }
        public static void TestSerialization()
        {
            string jsonString = File.ReadAllText(@"D:\SER\EInvoiceApp\Bin\Debug\NotSigned\729238687_299674_Source.json");
            Document doc = JsonConvert.DeserializeObject<Document>(jsonString);
            string jsonSerial = JsonConvert.SerializeObject(doc, new JsonSerializerSettings() { FloatFormatHandling = FloatFormatHandling.String, FloatParseHandling = FloatParseHandling.Decimal, DateFormatHandling = DateFormatHandling.IsoDateFormat, DateParseHandling = DateParseHandling.None ,Formatting = Formatting.Indented});
            File.WriteAllText(@"D:\729238687_299674_json_Serialized.json", jsonSerial);
            DocumentSigner signer = new DocumentSigner();
            string SerializedDoc = signer.Serialize(doc);
            string hisSerialization = File.ReadAllText(@"D:\SER\EInvoiceApp\Bin\Debug\Serialized\729238687_299674_CanonicalString.json");
            File.WriteAllText(@"D:\729238687_299674_Serialized.json", SerializedDoc);
            Console.WriteLine(SerializedDoc.Equals(hisSerialization,StringComparison.OrdinalIgnoreCase));
            Console.WriteLine($"Has Same Length: {SerializedDoc.Length==hisSerialization.Length}");
            using (var sha = SHA256.Create())
            {
                Console.WriteLine();
                byte[] serHash = sha.ComputeHash(Encoding.UTF8.GetBytes(SerializedDoc));
                byte[] hisHash = sha.ComputeHash(Encoding.UTF8.GetBytes(hisSerialization));
                Console.WriteLine(serHash.Length==hisHash.Length);
                for(int i = 0; i < serHash.Length; i++)
                {
                    Console.WriteLine($"{ serHash[i]} -- { hisHash[i]} -- {serHash[i]==hisHash[i]}");
                    Console.ReadLine();
                }
            }
            Console.ReadLine();
        }
        public static void TestTaxTypesDaoClass()
        {
            HomeController homeController = new HomeController();
            Settings settings = AppSettingsController.Settings;
            using (var connection = ObjectFactory.Connection)
            {
                ITaxTypeDao taxTypeDao = new TaxTypeDaoAdoImpl(connection);
                IList<TaxType> taxTypes = JsonConvert.DeserializeObject<IList<TaxType>>(File.ReadAllText(@"C:\Users\sp1\Desktop\EInvoice\SyncData\bin\Debug\TaxableTypes.json"));
                IList<TaxSubType> taxSubTypes = JsonConvert.DeserializeObject<IList<TaxSubType>>(File.ReadAllText(@"C:\Users\sp1\Desktop\EInvoice\SyncData\bin\Debug\taxsubtypes.json"));
                foreach(TaxSubType subType in taxSubTypes)
                {
                    var taxT = (from tt in taxTypes where tt.Code == subType.TaxtypeReference select tt).FirstOrDefault();
                    if(taxT!=null)
                        taxT.SubType.Add(subType);
                }
                taxTypeDao.AddRange(taxTypes);
            }
        }
        public static void TestGetNewDataFromOracle()
        {
            HomeController homeController = new HomeController();
            Settings settings = AppSettingsController.Settings;
            using (var connection = ObjectFactory.Connection)
            {
                ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
                IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection,taxableItemDao);
                IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
                IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
                IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
                IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
                Issuer issuer = issuerDao.Find("205018637");
                APIEnvironment environment = environmentDao.Find().Where(env => env.Id == 1).FirstOrDefault();
                IList<Document> docs = documentDao.GetNewDataFromOracle(issuer, environment);
                DocumentSigner documentSigner = new DocumentSigner();
                foreach(Document doc in docs)
                {
                    string serialized = documentSigner.Serialize(doc);
                    string jsonText = JsonConvert.SerializeObject(new { documents = new List<Document>() { doc } }, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ", Converters = new List<JsonConverter>() { new DecimalFormatConverter() } });
                    File.WriteAllText(@"C:\Users\sp1\Desktop\EInvoice\EInvoice.DesktopUI\bin\Debug\Serialized\"+doc.InternalId+".json", serialized);
                    File.WriteAllText(@"C:\Users\sp1\Desktop\EInvoice\EInvoice.DesktopUI\bin\Debug\Invoices\"+doc.InternalId+".json", jsonText);
                }
            }
        }
        public static void TestGetInvoiceSummary()
        {
            HomeController homeController = new HomeController();
            Settings settings = AppSettingsController.Settings;
            using (var connection = ObjectFactory.Connection)
            {
                ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
                IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
                IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
                IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
                IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
                IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
                Issuer issuer = issuerDao.Find("205018637");
                APIEnvironment environment = environmentDao.Find().Where(env => env.Id == 1).FirstOrDefault();
                var list = documentDao.GetInvoiceSummary(environment.Id.Value, issuer.Id, "%", null, null);
                foreach(InvoiceSummaryView invoiceSummary in list)
                {
                    Console.WriteLine($"{invoiceSummary.ProformaInvoiceNumber} Date: {invoiceSummary.DateTimeIssued.ToShortDateString()}");
                }
            }
        }
        public static void TestDocumentSubmissions()
        {
            HomeController homeController = new HomeController();
            DbConnection connection = ObjectFactory.Connection;
            ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
            IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
            IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
            IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
            IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
            Issuer issuer = issuerDao.Find("205018637");
            IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
            APIEnvironment environment = environmentDao.Find().FirstOrDefault();
            IList<Document> documents = documentDao.GetNewDataFromOracle(issuer,environment);
            if (documents.Count > 0)
            {
                documentDao.SaveOrUpdateDocument(documents[0]);
                DocumentSubmission submission = new DocumentSubmission()
                {
                    APIEnvironment = environment,
                    Document = documents[0],
                    Status = "Invalid",
                    SubmissionDate = DateTime.UtcNow,
                    SubmissionUUID = "---------------------------",
                    UUID = "-----------------------------------------"
                };
                documentDao.SaveOrUpdateDocumentSubmission(submission);
            }
        }
        public static void TestFindDocInOracleByInternalId()
        {
            IList<Document> docs = ObjectFactory.DocumentDao.FindDocumentInOracleByInternalId("306022", ObjectFactory.IssuerDao.Find("100507042"));
            if (docs.Count == 1)
            {
                Console.WriteLine("Found Document");
                Console.WriteLine(docs[0].ProformaInvoiceNumber);
            }
        }
        static void Main(string[] args)
        {

            //HomeController homeController = new HomeController();
            //DbConnection connection = ObjectFactory.CreateConnection(AppSettingsController.Settings);
            //using (var connection = ObjectFactory.Connection)
            //{
            //   LoadActivityCode(connection);
            //  LoadCountryCodes(connection);
            //}
            //Sync_Old_DataBase_Engineering_With_Portal_Production("ENGTAXSRV");
            SyncDBWithAPI(ObjectFactory.CreateConnection(AppSettingsController.Settings));
            //SyncDBWithAPIUsingPackageRequest(connection);
            //SyncDatabaseWithPortalUsingSpecifiedRequests();
            //if (connection.State != ConnectionState.Open)
            //     connection.Open();
            //SyncDBWithAPI(connection);
            // if (connection.State != ConnectionState.Closed)
            //      connection.Close();
            //
            //PutPackageRequests(new List<string>(){ "310700655", "100507042", "205018637" });
            //TestSerialization();
            //SyncDatabaseWithPortalUsingSpecifiedRequests();
            //IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
            //APIEnvironment environment = environmentDao.Find().FirstOrDefault();
            //IIssuerAPIAccessDetailsDao issuerAPIAccess = new IssuerAPIAccessDetailsDaoAdoImpl(connection);
            //IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
            //Issuer issuer = issuerDao.Find("100507042");
            //var details = issuerAPIAccess.Find(environment, issuer);
            //IEInvoiceAPIProxy proxy = new EInvoiceAPIRestSharpProxy(environment,details.ClientId,details.ClientSecret);
            //File.WriteAllBytes(@"d:\2M6QH8RZ6EBPR1TVVJ47D9GF10.pdf", proxy.GetDocumentPrintOut("2M6QH8RZ6EBPR1TVVJ47D9GF10"));
            //TestTaxTypesDaoClass();
            //TestGetNewDataFromOracle();
            //TestGetInvoiceSummary();
            //TestDocumentSubmissions();
            //TestSerialization();
            //TryFakeCert();
            //CompareTwoSerializationMethods();
            //
            //TestFindDocInOracleByInternalId();
            //TestSubmit();
            Console.ReadLine();
        }
        public static void LoadActivityCode(DbConnection connection)
        {
            IActivityCodeDao activityCodeDao = new ActivityCodeDaoAdoImpl(connection);
            IList<ActivityType> activities = JsonConvert.DeserializeObject<IList<ActivityType>>(File.ReadAllText("ActivityCodes.json"));
            activityCodeDao.AddRange(activities);
        }
        public static void LoadCountryCodes(DbConnection connection)
        {
            ICountryCodeDao countryCodeDao = new CountryCodeDaoAdoImpl(connection);
            IList<CountryCode> countryCodes = JsonConvert.DeserializeObject<IList<CountryCode>>(File.ReadAllText("CountryCodes.json"));
            countryCodeDao.AddRange(countryCodes);
        }
        public static void SyncDatabaseWithPortalUsingSpecifiedRequests()
        {
            HomeController homeController = new HomeController();
            Settings settings = AppSettingsController.Settings;
            using (DbConnection connection = ObjectFactory.Connection)
            {
                IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
                Issuer issuer_eng = issuerDao.Find("100507042");
                Issuer issuer_elec = issuerDao.Find("205018637");
                Issuer issuer_home = issuerDao.Find("310700655");
                IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
                IList<APIEnvironment> environments = environmentDao.Find();
                APIEnvironment environment_PREPROD = (from env in environments where env.Id == 1 select env).FirstOrDefault();
                APIEnvironment environment_PROD = (from env in environments where env.Id == 2 select env).FirstOrDefault();
                IList<string> DonePackages = new List<string>();
                int ProcessedPackageCount = 0;
                do
                {
                    
                    try
                    {
                        if (!DonePackages.Contains("121460"))
                        {
                            GetLostDocumentsFromPortal(issuer_eng, environment_PREPROD, "121460", connection);
                            ProcessedPackageCount++;
                            DonePackages.Add("121460");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    try
                    {
                        if (!DonePackages.Contains("75480"))
                        { 
                            GetLostDocumentsFromPortal(issuer_eng, environment_PROD, "75480", connection);
                            DonePackages.Add("75480");
                            ProcessedPackageCount++;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    try
                    {
                        if (!DonePackages.Contains("121461"))
                        {
                            GetLostDocumentsFromPortal(issuer_elec, environment_PREPROD, "121461", connection);
                            DonePackages.Add("121461");
                            ProcessedPackageCount++;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    try
                    {
                        if (!DonePackages.Contains("75481"))
                        {
                            GetLostDocumentsFromPortal(issuer_elec, environment_PROD, "75481", connection);
                            DonePackages.Add("75481");
                            ProcessedPackageCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    
                    try
                    {
                        if (!DonePackages.Contains("121459"))
                        {
                            GetLostDocumentsFromPortal(issuer_home, environment_PREPROD, "121459", connection);
                            DonePackages.Add("121459");
                            ProcessedPackageCount++;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    
                    try
                    {
                        if (!DonePackages.Contains("75479"))
                        {
                            GetLostDocumentsFromPortal(issuer_home, environment_PROD, "75479", connection);
                            DonePackages.Add("75479");
                            ProcessedPackageCount++;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    Console.WriteLine($"Completed So Far { DonePackages.Count} / 6 .");
                } while (ProcessedPackageCount < 6);
            }
        }
        public static void GetLostDocumentsFromPortal(Issuer issuer,APIEnvironment environment,string packgeRequestId,DbConnection connection)
        {
            IIssuerAPIAccessDetailsDao detailsDao = new IssuerAPIAccessDetailsDaoAdoImpl(connection);
            ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
            IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
            IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
            IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
            var details = detailsDao.Find(environment, issuer);
            if (details != null)
            {
                EInvoiceAPIRestSharpProxy proxy = new EInvoiceAPIRestSharpProxy(environment, details.ClientId, details.ClientSecret);
                Console.WriteLine($"Download Package: {packgeRequestId}. This may take a while.");
                if (!Directory.Exists(Environment.CurrentDirectory + "\\Packages_compressed"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\Packages_compressed");
                }
                if (!File.Exists(Environment.CurrentDirectory + "\\Packages_compressed\\" + packgeRequestId + ".zip"))
                {
                    byte[] packageData = proxy.GetDocumentPackage(packgeRequestId);
                    File.WriteAllBytes(Environment.CurrentDirectory + "\\Packages_compressed\\" + packgeRequestId + ".zip", packageData);
                }
                if (!Directory.Exists(Environment.CurrentDirectory + "\\Packages_extracted\\"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\Packages_extracted");
                }
                Console.WriteLine($"Done Downloading Package {packgeRequestId} ...");
                Console.WriteLine("Extracting Received Package....this may take a while.");
                if (!Directory.Exists(Environment.CurrentDirectory + "\\Packages_extracted\\" + packgeRequestId))
                {
                    ZipArchive archive = ZipFile.OpenRead(Environment.CurrentDirectory + "\\Packages_compressed\\" + packgeRequestId + ".zip");
                    archive.ExtractToDirectory(Environment.CurrentDirectory + "\\Packages_extracted\\" + packgeRequestId);
                }
                string[] allfiles = Directory.GetFiles(Environment.CurrentDirectory + "\\Packages_extracted\\" + packgeRequestId, "*.json", SearchOption.AllDirectories).Where(s => !s.Contains("metadata")).ToArray();
                JsonSerializer serializer = new JsonSerializer();
                Console.WriteLine($"Total Files: {allfiles.Length}");
                int fileCounter = 0;
                double percenatge = 0;
                foreach (string file in allfiles)
                {
                    using (var reader = File.OpenText(file))
                    {
                        SignedDocument doc = serializer.Deserialize<SignedDocument>(new JsonTextReader(reader));
                        if (doc.Issuer != null && doc.Issuer.Id == issuer.Id)
                        {
                            IList<Document> foundDocs = documentDao.FindByInternalId(doc.InternalId);
                            if (foundDocs.Count == 0)
                            {
                                var docExtended = proxy.GetDocument(doc.taxAuthorityDocument.uuid);
                                var tempDoc = docExtended.taxAuthorityDocument;
                                var rec_id = receiverDao.FindReceiverId(docExtended.receiverName);
                                if (rec_id == null)
                                    receiverDao.Insert(tempDoc.Receiver);
                                else
                                    tempDoc.Receiver.InternalId = rec_id;
                                Console.WriteLine($"{ tempDoc.Receiver.InternalId}--{tempDoc.Receiver.Name}");
                                documentDao.Insert(tempDoc);
                                documentDao.InsertDocumentSubmission(new DocumentSubmission
                                {
                                    Document = tempDoc,
                                    APIEnvironment = environment,
                                    Status = docExtended.status,
                                    SubmissionDate = (DateTime)docExtended.dateTimeReceived,
                                    SubmissionUUID = docExtended.submissionUUID,
                                    UUID = docExtended.uuid
                                });
                                Console.WriteLine($"Document#{tempDoc.ProformaInvoiceNumber} Date:{tempDoc.DateTimeIssued.ToShortDateString()}");
                            }
                            else
                            {
                                if (!documentDao.IsDocumentSubmissionExists(foundDocs[0].Id, environment.Id))
                                {
                                    var docExtended = proxy.GetDocument(doc.taxAuthorityDocument.uuid);
                                    documentDao.SaveOrUpdateDocumentSubmission(new DocumentSubmission()
                                    {
                                        APIEnvironment = environment,
                                        Document = foundDocs[0],
                                        Status = docExtended.status,
                                        SubmissionDate = (DateTime)docExtended.dateTimeReceived,
                                        SubmissionUUID = docExtended.submissionUUID,
                                        UUID = docExtended.uuid
                                    });
                                    Console.WriteLine($"Document Submission#{docExtended.submissionUUID} Date:{((DateTime)docExtended.dateTimeReceived).ToShortDateString()}");
                                }
                            }
                        }
                    }
                    fileCounter++;
                    if (percenatge != ((double)fileCounter / (double) allfiles.Length * 100.0))
                    {
                        percenatge = ((double)fileCounter / (double)allfiles.Length * 100.0);
                        int left = Console.CursorLeft;
                        int top = Console.CursorTop;
                        Console.SetCursorPosition(50, 0);
                        Console.Write($"{percenatge} % .");
                        Console.SetCursorPosition(left, top);
                    }
                    
                }
            }
                        
        }
        public static void SyncDBWithAPIUsingPackageRequest(DbConnection connection) 
        {
            IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
            IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
            IIssuerAPIAccessDetailsDao detailsDao = new IssuerAPIAccessDetailsDaoAdoImpl(connection);
            ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
            IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
            IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
            IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
            IList<Issuer> issuers = issuerDao.Find();
            if(Directory.Exists(Environment.CurrentDirectory + "\\Packages_extracted"))
                Directory.Delete(Environment.CurrentDirectory + "\\Packages_extracted", true);
            foreach (Issuer issuer in issuers)
            {
                IList<APIEnvironment> environments = environmentDao.Find();
                Console.WriteLine($"Current Taxpayer: {issuer.Name}");
                foreach (APIEnvironment environment in environments)
                {
                    //if (environment.Name == "Production")
                    //{
                        Console.WriteLine(environment.Name);
                        Console.WriteLine($"Current API: {environment.Name}");
                        var accessDetails = detailsDao.Find(environment, issuer);
                        if (accessDetails == null)
                            break;
                        IEInvoiceAPIProxy proxy = new EInvoiceAPIRestSharpProxy(environment, accessDetails.ClientId, accessDetails.ClientSecret);
                        var result = proxy.GetPackageRequests(1, 100);
                        var found = (from itm in result.result where itm.status == 2 || itm.status == 1 && itm.format == 3 && itm.isExpired == false select itm).ToList();
                        if (found.Count == 0)
                        {
                            proxy.RequestDocumentPackage(new DocumentPackageRequest
                            {
                                format = "JSON",
                                type = "Full",
                                queryParameters = new QueryParameters()
                                {
                                    dateFrom = DateTime.Parse("2021-05-15T00:00:00Z"),
                                    dateTo = DateTime.Parse("2021-06-25T23:59:00Z"),
                                    statuses = new List<string>() { "Valid" }
                                }
                            });
                        }
                        do
                        {
                            result = proxy.GetPackageRequests(1, 100);
                            found = (from itm in result.result where itm.status == 2 && itm.format == 3 && itm.isExpired == false select itm).ToList();
                            if (found.Count == 0)
                            {
                                Console.WriteLine("Packge Request Is not ready.");
                                goto label;
                            }

                            foreach (DocumentPackageInformation pkg in found)
                            {
                                Console.WriteLine($"Download Package: {pkg.packageId}. This may take a while.");
                                if (!File.Exists(Environment.CurrentDirectory + "\\Packages_compressed\\" + pkg.packageId + ".zip"))
                                {
                                    byte[] data = proxy.GetDocumentPackage(pkg.packageId);
                                    File.WriteAllBytes(Environment.CurrentDirectory + "\\Packages_compressed\\" + pkg.packageId + ".zip", data);
                                }
                                Console.WriteLine("Done Downloading Package...");
                                if (!Directory.Exists(Environment.CurrentDirectory + "\\Packages_compressed"))
                                {
                                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\Packages_compressed");
                                }

                                ZipArchive archive = ZipFile.OpenRead(Environment.CurrentDirectory + "\\Packages_compressed\\" + pkg.packageId + ".zip");
                                if (!Directory.Exists(Environment.CurrentDirectory + "\\Packages_extracted\\" + DateTime.Now.ToShortTimeString()))
                                {

                                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\Packages_extracted");
                                }

                                archive.ExtractToDirectory(Environment.CurrentDirectory + "\\Packages_extracted");
                                string[] allfiles = Directory.GetFiles(Environment.CurrentDirectory + "\\Packages_extracted", "*.json", SearchOption.AllDirectories);
                                Console.WriteLine($"Total Invoice = {allfiles.Length}");
                                JsonSerializer serializer = new JsonSerializer();
                                foreach (string file in allfiles)
                                {
                                    using (var reader = File.OpenText(file))
                                    {
                                        SignedDocument doc = serializer.Deserialize<SignedDocument>(new JsonTextReader(reader));
                                        Console.WriteLine(doc?.ProformaInvoiceNumber);
                                    if (doc.Issuer != null && doc.Issuer.Id == issuer.Id)
                                    {
                                        IList<Document> foundDocs = documentDao.FindByInternalId(doc.InternalId);
                                        //var docExtended = proxy.GetDocument(doc.taxAuthorityDocument.uuid);
                                        try
                                        {
                                            if (foundDocs.Count == 0)
                                            {
                                                var docExtended = proxy.GetDocument(doc.taxAuthorityDocument.uuid);
                                                var tempDoc = docExtended.taxAuthorityDocument;
                                                documentDao.Insert(tempDoc);
                                                documentDao.InsertDocumentSubmission(new DocumentSubmission
                                                {
                                                    Document = tempDoc,
                                                    APIEnvironment = environment,
                                                    Status = docExtended.status,
                                                    SubmissionDate = (DateTime)docExtended.dateTimeReceived,
                                                    SubmissionUUID = docExtended.submissionUUID,
                                                    UUID = docExtended.uuid
                                                });
                                                Console.WriteLine($"Document#{tempDoc.ProformaInvoiceNumber} Date:{tempDoc.DateTimeIssued.ToShortDateString()}");
                                                Console.WriteLine($"Document#{doc.ProformaInvoiceNumber} Date:{doc.DateTimeIssued.ToShortDateString()}");
                                            }
                                            else
                                            {
                                                if (!documentDao.IsDocumentSubmissionExists(foundDocs[0].Id, environment.Id))
                                                {
                                                    var docExtended = proxy.GetDocument(doc.taxAuthorityDocument.uuid);
                                                    documentDao.SaveOrUpdateDocumentSubmission(new DocumentSubmission()
                                                    {
                                                        APIEnvironment = environment,
                                                        Document = foundDocs[0],
                                                        Status = docExtended.status,
                                                        SubmissionDate = (DateTime)docExtended.dateTimeReceived,
                                                        SubmissionUUID = docExtended.submissionUUID,
                                                        UUID = docExtended.uuid
                                                    });
                                                }
                                            }
                                        }
                                        catch(WebAPIException ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                        }
                                    }
                                }
                                break;
                            }
                        } while (found.Count == 0);
                    //}
                label:;
                }
            }
        }
        public static void SyncDBWithAPI(DbConnection connection)
        {
            Console.WriteLine("******* Database Sync With API **************");
            //DbConnection connection = new SqlConnection(connectionString);
            IIssuerDao issuerDao = new IssuerDaoAdoImpl(connection);
            IAPIEnvironmentDao environmentDao = new APIEnvironmentDaoAdoImpl(connection);
            IIssuerAPIAccessDetailsDao detailsDao = new IssuerAPIAccessDetailsDaoAdoImpl(connection);
            ITaxableItemDao taxableItemDao = new TaxableItemDaoAdoImpl(connection);
            IInvoiceLineDao invoiceLineDao = new InvoiceLineDaoAdoImpl(connection, taxableItemDao);
            IReceiverDao receiverDao = new ReceiverDaoAdoImpl(connection);
            IDocumentDao documentDao = new DocumentDaoAdoImpl(connection, invoiceLineDao, receiverDao);
            IList<Issuer> issuers = issuerDao.Find().ToList();
            foreach (Issuer issuer in issuers)
            {
                IList<APIEnvironment> environments = environmentDao.Find().ToList();
                Console.WriteLine($"Current Taxpayer: {issuer.Name}");
                foreach (APIEnvironment environment in environments)
                {
                    Console.WriteLine($"Current API: {environment.Name}");
                    var accessDetails = detailsDao.Find(environment, issuer);
                    if (accessDetails == null)
                        break;
                    IEInvoiceAPIProxy proxy = new EInvoiceAPIRestSharpProxy(environment, accessDetails.ClientId, accessDetails.ClientSecret);
                    RecentDocumentsResult result = null;
                    int currentPage = 1;
                    int soFar = 0;
                    do
                    {
                        result = proxy.GetRecentDocuments(currentPage, 100);
                        foreach (var doc in result.result)
                        {
                            if (doc.status == "Valid" && doc.issuerId == issuer.Id)
                            {
                                
                                IList<Document> found = documentDao.FindByInternalId(doc.internalId);
                                var docExtended = proxy.GetDocument(doc.uuid);
                                if (found.Count == 0)
                                {
                                    var tempDoc = docExtended.taxAuthorityDocument;
                                    if (!Directory.Exists(Environment.CurrentDirectory + "\\SyncDocuments"))
                                    {
                                        Directory.CreateDirectory(Environment.CurrentDirectory + "\\SyncDocuments");
                                    }
                                   
                                    File.WriteAllText(Environment.CurrentDirectory+"\\SyncDocuments\\"+docExtended.issuerId+"_"+docExtended.taxAuthorityDocument.ProformaInvoiceNumber+".json", docExtended.document,Encoding.UTF8);
                                    tempDoc.Receiver.InternalId = receiverDao.FindReceiverId(tempDoc.Receiver.Name);
                                    if (tempDoc.Receiver.InternalId == null)
                                    {
                                        receiverDao.Insert(tempDoc.Receiver);
                                    }
                                    documentDao.Insert(tempDoc);
                                    documentDao.InsertDocumentSubmission(new DocumentSubmission 
                                    { 
                                        Document = tempDoc,
                                        APIEnvironment = environment,
                                        Status = docExtended.status,
                                        SubmissionDate = (DateTime)docExtended.dateTimeReceived,
                                        SubmissionUUID = docExtended.submissionUUID,
                                        UUID = docExtended.uuid
                                    });
                                    Console.WriteLine($"Document#{tempDoc.ProformaInvoiceNumber} Date:{tempDoc.DateTimeIssued.ToShortDateString()}");
                                }
                                else
                                {
                                    if (!documentDao.IsDocumentSubmissionExists(found[0].Id, environment.Id))
                                    {
                                        documentDao.SaveOrUpdateDocumentSubmission(new DocumentSubmission()
                                        {
                                            APIEnvironment = environment,
                                            Document = found[0],
                                            Status = docExtended.status,
                                            SubmissionDate = (DateTime)docExtended.dateTimeReceived,
                                            SubmissionUUID = docExtended.submissionUUID,
                                            UUID = docExtended.uuid
                                        });
                                    }
                                }
                            }
                        }
                        currentPage++;
                        soFar += 100;
                        Console.WriteLine($"Processed So Far {soFar}/{result.metadata.totalCount} .");
                    } while (currentPage < result.metadata.totalPages);
                }
            }
            Console.WriteLine("Press Enter To Exit....");
            Console.ReadLine();
        }
    }
}
