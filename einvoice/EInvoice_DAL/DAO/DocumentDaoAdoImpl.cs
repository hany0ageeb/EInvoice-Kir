using System.Data;
using System.Linq;
using System.Data.Common;
using EInvoice.Model;
using System.Collections.Generic;
using System;

namespace EInvoice.DAL.DAO
{
    public class DocumentDaoAdoImpl : IDocumentDao
    {
        private readonly DbConnection _connection;
        private readonly IInvoiceLineDao _invoiceLineDao;
        private readonly IReceiverDao _receiverDao;
        public DocumentDaoAdoImpl(DbConnection conn,IInvoiceLineDao invoiceLineDao,IReceiverDao receiverDao)
        {
            _connection = conn;
            _invoiceLineDao = invoiceLineDao;
            _receiverDao = receiverDao;
        }
        public IList<InvoiceSummaryView> GetInvoiceSummary(int APIEnvironmentId,string IssuerId,string ReceiverName = "%",DateTime? issueDateFrom = null,DateTime? issueDateTo=null)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[FindInvoiceSummary]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter("@APIEnvironmentId", parameterValue: APIEnvironmentId, dbType: DbType.Int32));
            selectCommand.Parameters.Add(selectCommand.CreateParameter("@TaxpayerId", parameterValue: IssuerId, dbType: DbType.String));
            selectCommand.Parameters.Add(selectCommand.CreateParameter("@CustomerName", parameterValue: ReceiverName, dbType: DbType.String));
            selectCommand.Parameters.Add(selectCommand.CreateParameter("@IssueDateFrom", parameterValue: issueDateFrom, dbType: DbType.DateTime));
            selectCommand.Parameters.Add(selectCommand.CreateParameter("@IssueDateTo", parameterValue: issueDateTo, dbType: DbType.DateTime));
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                DbDataReader reader = selectCommand.ExecuteReader();
                IList<InvoiceSummaryView> invoices = new List<InvoiceSummaryView>();
                while (reader.Read())
                {
                    InvoiceSummaryView invoice = new InvoiceSummaryView();
                    invoice.DateTimeIssued = Convert.ToDateTime(reader["DateTimeIssued"]);
                    invoice.IssuerName = Convert.ToString(reader["ISS_Name"]);
                    invoice.NetAmount = Convert.ToDecimal(reader["NetAmount"]);
                    invoice.ProformaInvoiceNumber = Convert.ToString(reader["ProformaInvoiceNumber"]);
                    invoice.ReceiverName = Convert.ToString(reader["RECV_Name"]);
                    invoice.Status = Convert.ToString(reader["Status"]);
                    invoice.TableTax = Convert.ToDecimal(reader["TableTax"]);
                    invoice.TotalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                    invoice.TotalDiscountAmount = Convert.ToDecimal(reader["TotalDiscountAmount"]);
                    invoice.TotalSalesAmount = Convert.ToDecimal(reader["TotalSalesAmount"]);
                    invoice.UUID = Convert.ToString(reader["UUID"]);
                    invoice.ValueAddedTax = Convert.ToDecimal(reader["ValueAddedTax"]);
                    invoice.WithHoldingTax = Convert.ToDecimal(reader["WithHoldingTax"]);
                    invoices.Add(invoice);
                }
                reader.Close();
                _connection.Close();
                return invoices;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }
        public IList<DocumentExtended> FindDocumentSubmissions(
            DateTime? issueDateFrom,
            DateTime? issueDateTo,
            DateTime? submissionDateFrom,
            DateTime? submissionDateTo,
            Issuer issuer,
            int APIEnvironmentId,
            Receiver receiver,
            string proformaInvoiceNumber,
            string status
        )
        {
            DbCommand command = _connection.CreateCommand("[dbo].[SearchDocuments]", CommandType.StoredProcedure);
            if(issueDateFrom != null)
                command.Parameters.Add(command.CreateParameter(parameterName: "@issuanceDateFrom", parameterValue: issueDateFrom,dbType:DbType.DateTime));
            if (issueDateTo != null)
                command.Parameters.Add(command.CreateParameter(parameterName: "@issuanceDateTo", parameterValue: issueDateTo, dbType: DbType.DateTime));
            if(submissionDateFrom!=null)
                command.Parameters.Add(command.CreateParameter(parameterName: "@submissionDateFrom", parameterValue: submissionDateFrom, dbType: DbType.DateTime));
            if(submissionDateTo!=null)
                command.Parameters.Add(command.CreateParameter(parameterName: "@submissionDateTo", parameterValue: submissionDateTo, dbType: DbType.DateTime));
            if (receiver != null && receiver.InternalId > 0)
                command.Parameters.Add(command.CreateParameter(parameterName: "@CustomerName", parameterValue:receiver.Name,dbType:DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TaxpayerId", parameterValue: issuer.Id, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@APIEnvironmentId", parameterValue: APIEnvironmentId, dbType: DbType.Int32));
            if (!string.IsNullOrEmpty(proformaInvoiceNumber) && status != "ALL")
                command.Parameters.Add(command.CreateParameter(parameterName: "@status", parameterValue:status,dbType:DbType.String));
            if (!string.IsNullOrEmpty(proformaInvoiceNumber))
                command.Parameters.Add(command.CreateParameter(parameterName: "@ProformaInvoiceNumber",parameterValue:proformaInvoiceNumber,dbType:DbType.String));
            DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
            adapter.SelectCommand = command;
            DataTable docsTable = new DataTable("Seacrh Result");
            adapter.Fill(docsTable);
            var found = docsTable.AsEnumerable();
            IList<DocumentExtended> documents = new List<DocumentExtended>();
            foreach(var row in found)
            {
                DocumentExtended doc = new DocumentExtended();
                doc.dateTimeIssued = row.Field<DateTime>("DateTimeIssued");
                doc.dateTimeReceived = row.Field<DateTime?>("SubmissionDate");
                doc.internalId = row.Field<string>("InternalId");
                doc.issuerId = row.Field<string>("TaxpayerId");
                doc.issuerName = row.Field<string>("ISS_Name");
                doc.netAmount = row.Field<decimal>("NetAmount");
                doc.receiverId = row.Field<string>("RECV_Id");
                doc.receiverName = row.Field<string>("RECV_Name");
                doc.status = row.Field<string>("Status");
                doc.submissionUUID = row.Field<string>("SubmissionUUID");
                doc.total = row.Field<decimal>("TotalAmount");
                doc.uuid = row.Field<string>("UUID");
                doc.InvoiceNumber = row.Field<string>("ProformaInvoiceNumber");
                doc.taxAuthorityDocument = new Document();
                doc.taxAuthorityDocument.ProformaInvoiceNumber = row.Field<string>("ProformaInvoiceNumber");
                documents.Add(doc);
            }
            return documents;
        }
        public IList<DocumentSubmission> FindDocumentSubmissionsByState(Issuer issuer, APIEnvironment env, string state)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[FindDocumentSubmissionsByState]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@State",parameterValue:state,dbType:DbType.String));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@IssuerId", parameterValue:issuer.Id, dbType:DbType.Int32));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@APIEnvId",parameterValue:env.Id,dbType:DbType.Int32));
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            DbDataReader reader = selectCommand.ExecuteReader();
            IList<DocumentSubmission> submissions = new List<DocumentSubmission>();
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>()
            {
                { "DocumentSubmission.SubmissionUUID","SubmissionUUID" },
                { "DocumentSubmission.UUID","UUID" },
                { "DocumentSubmission.Status","Status" },
                { "DocumentSubmission.SubmissionDate","SubmissionDate" },
                { "DocumentSubmission.Version","VerCol"},
                { "APIEnvironment.Id","APIEnvironmentId" },
                { "APIEnvironment.Name","APIENV_Name" },
                { "APIEnvironment.BaseUri","APIENV_BaseUri" },
                { "APIEnvironment.LogInUri","APIENV_LogInUri" },
                { "APIEnvironment.Version","APIENV_VerCol" },
                { "Document.DateTimeIssued","DateTimeIssued" },
                { "Document.DocumentType","DocumentType" },
                { "Document.DocumentTypeVersion","DocumentTypeVersion" },
                { "Document.ExtraDiscountAmount","ExtraDiscountAmount" },
                { "Document.InternalId","InternalId" },
                { "Document.Id","DocumentId" },
                { "Document.NetAmount","NetAmount" },
                { "Document.ProformaInvoiceNumber","ProformaInvoiceNumber" },
                { "Document.PurchaseOrderDescription","PurchaseOrderDescription" },
                { "Document.PurchaseOrderReference","PurchaseOrderReference" },
                { "Document.SalesOrderDescription","SalesOrderDescription" },
                { "Document.SalesOrderReference","SalesOrderReference" },
                { "Document.TaxpayerActivityCode","TaxpayerActivityCode" },
                { "Document.TotalAmount","TotalAmount" },
                { "Document.TotalSalesAmount","TotalSalesAmount" },
                { "Document.TotalDiscountAmount","TotalDiscountAmount" },
                { "Document.Version","DOC_VerCol" },
                { "Document.TotalItemsDiscountAmount","TotalItemsDiscountAmount" },
                { "Delivery.Approach","Approach" },
                { "Delivery.CountryOfOrigin","CountryOfOrigin" },
                { "Delivery.DateValidity","DateValidity" },
                { "Delivery.ExportPort","ExportPort" },
                { "Delivery.GrossWeight","GrossWeight" },
                { "Delivery.NetWeight","NetWeight" },
                { "Delivery.Packaging","Packaging" },
                { "Delivery.Terms","DeliveryTerms" },
                { "Issuer.Id","TaxpayerId" },
                { "Issuer.Name","ISS_Name" },
                { "Issuer.Type","ISS_Type" },
                { "Issuer.Version","ISS_VerCol" },
                { "IssuerAddress.AdditionalInformation","ISS_AdditionalInformation" },
                { "IssuerAddress.BranchId","ISS_BranchId" },
                { "IssuerAddress.BuildingNumber","ISS_BuildingNumber" },
                { "IssuerAddress.Country","ISS_Country" },
                { "IssuerAddress.Floor","ISS_Floor" },
                { "IssuerAddress.Governate","ISS_Governate" },
                { "IssuerAddress.Landmark","ISS_Landmark" },
                { "IssuerAddress.PostalCode","ISS_PostalCode" },
                { "IssuerAddress.RegionCity","ISS_RegionCity" },
                { "IssuerAddress.Room","ISS_Room" },
                { "IssuerAddress.Street","ISS_Street" },
                { "Receiver.Id","RECV_Id" },
                { "Receiver.InternalId","CustomerId" },
                { "Receiver.Name","RECV_Name" },
                { "Receiver.Type","RECV_Type" },
                { "Receiver.Version","RECV_VerCol" },
                { "ReceiverAddress.AdditionalInformation","REC_AdditionalInformation" },
                { "ReceiverAddress.BuildingNumber","REC_BuildingNumber" },
                { "ReceiverAddress.Country","REC_Country" },
                { "ReceiverAddress.Floor","REC_Floor" },
                { "ReceiverAddress.Governate","REC_Governate" },
                { "ReceiverAddress.Landmark","RECV_Landmark" },
                { "ReceiverAddress.PostalCode","RECV_PostalCode" },
                { "ReceiverAddress.RegionCity","RECV_RegionCity" },
                { "ReceiverAddress.Room","RECV_Room" },
                { "ReceiverAddress.Street","RECV_Street" },
                { "Payment.BankAccountIBAN","BankAccountIBAN" },
                { "Payment.BankAccountNo","BankAccountNo" },
                { "Payment.BankAddress","BankAddress" },
                { "Payment.BankName","BankName" },
                { "Payment.SwiftCode","SwiftCode" },
                { "Payment.Terms","PaymentTerms" }
            };
            while (reader.Read())
            {
                DocumentSubmission submission = reader.ReadDocumentSubmission(propertyColumnMappings);
                submission.Document.InvoiceLines = _invoiceLineDao.FindByDocumentId(submission.Document.Id);
                submissions.Add(submission);
            }
            reader.Close();
            _connection.Close();
            return submissions;
            
        }
        public bool IsDocumentSubmissionExists(int? docId,int? EnvId)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetDocumentSubmissionCount]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@EnvId", parameterValue: EnvId, dbType:DbType.Int32));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@DocId", parameterValue: docId, dbType: DbType.Int32));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@Count", direction: ParameterDirection.Output, dbType: DbType.Int32));
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            selectCommand.ExecuteNonQuery();
            int count = (int)selectCommand.Parameters["@Count"].Value;
            _connection.Close();
            return count > 0;
        }
        public void InsertDocumentSubmission(DocumentSubmission submission)
        {
            DbCommand insertCommand = _connection.CreateCommand("[dbo].[InsertDocumentSubmission]", CommandType.StoredProcedure);
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@APIEnvironmentId", parameterValue:submission.APIEnvironment.Id,dbType:DbType.Int32));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DocumentId", parameterValue: submission.Document.Id, dbType: DbType.Int32));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@UUID", parameterValue: submission.UUID, dbType: DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@SubmissionUUID", parameterValue: submission.SubmissionUUID, dbType: DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@SubmissionDate", parameterValue: submission.SubmissionDate, dbType: DbType.DateTime));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Status", parameterValue: submission.Status, dbType: DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName:"@VerCol",dbType:DbType.Binary,direction:ParameterDirection.Output));
            insertCommand.Parameters["@VerCol"].Size = 8;
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                insertCommand.Connection = _connection;
                insertCommand.ExecuteNonQuery();
                submission.Version = insertCommand.Parameters["@VerCol"].Value as byte[];
            }
            finally
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Close();
                _connection.Close();
            }
        }
        public IList<Document> Find()
        {
            throw new NotImplementedException();
        }
        public IList<Document> FindByInternalId(string internalId)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetDocumentByInternalId]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@InternalId", parameterValue: internalId));
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            IList<Document> documents = new List<Document>();
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>() 
            {
                { "Document.DateTimeIssued","DateTimeIssued" },
                { "Document.DocumentType","DocumentType" },
                { "Document.DocumentTypeVersion","DocumentTypeVersion" },
                { "Document.ExtraDiscountAmount","ExtraDiscountAmount" },
                { "Document.InternalId","InternalId" },
                { "Document.Id","Id" },
                { "Document.NetAmount","NetAmount" },
                { "Document.ProformaInvoiceNumber","ProformaInvoiceNumber" },
                { "Document.PurchaseOrderDescription","PurchaseOrderDescription" },
                { "Document.PurchaseOrderReference","PurchaseOrderReference" },
                { "Document.SalesOrderDescription","SalesOrderDescription" },
                { "Document.SalesOrderReference","SalesOrderReference" },
                { "Document.TaxpayerActivityCode","TaxpayerActivityCode" },
                { "Document.TotalAmount","TotalAmount" },
                { "Document.TotalSalesAmount","TotalSalesAmount" },
                { "Document.TotalDiscountAmount","TotalDiscountAmount" },
                { "Document.Version","VerCol" },
                { "Document.TotalItemsDiscountAmount","TotalItemsDiscountAmount" },
                { "Issuer.Id","TaxpayerId" },
                { "Issuer.Name","ISS_Name" },
                { "Issuer.Type","ISS_Type" },
                { "Issuer.Version","ISS_VerCol" },
                { "IssuerAddress.AdditionalInformation","ISS_AdditionalInformation" },
                { "IssuerAddress.BranchId","ISS_BranchId" },
                { "IssuerAddress.BuildingNumber","ISS_BuildingNumber" },
                { "IssuerAddress.Country","ISS_Country" },
                { "IssuerAddress.Floor","ISS_Floor" },
                { "IssuerAddress.Governate","ISS_Governate" },
                { "IssuerAddress.Landmark","ISS_Landmark" },
                { "IssuerAddress.PostalCode","ISS_PostalCode" },
                { "IssuerAddress.RegionCity","ISS_RegionCity" },
                { "IssuerAddress.Room","ISS_Room" },
                { "IssuerAddress.Street","ISS_Street" },
                {"Delivery.Approach","Approach" },
                { "Delivery.CountryOfOrigin","CountryOfOrigin" },
                { "Delivery.DateValidity","DateValidity" },
                { "Delivery.ExportPort","ExportPort" },
                { "Delivery.GrossWeight","GrossWeight" },
                { "Delivery.NetWeight","NetWeight" },
                { "Delivery.Packaging","Packaging" },
                { "Delivery.Terms","DeliveryTerms" },
                {"Payment.BankAccountIBAN","BankAccountIBAN" },
                {"Payment.BankAccountNo","BankAccountNo" },
                {"Payment.BankAddress","BankAddress" },
                {"Payment.BankName","BankName" },
                {"Payment.SwiftCode","SwiftCode" },
                {"Payment.Terms","PaymentTerms" },
                {"Receiver.InternalId","CustomerId" },
                {"Receiver.Id","RECV_Id" },
                {"Receiver.Name","RECV_Name" },
                {"Receiver.Type","RECV_Type" },
                {"Receiver.Version","RECV_VerCol" },
                {"ReceiverAddress.AdditionalInformation","RECV_AdditionalInformation" },
                {"ReceiverAddress.BuildingNumber","RECV_BuildingNumber" },
                {"ReceiverAddress.Country","RECV_Country" },
                {"ReceiverAddress.Floor","RECV_Floor" },
                {"ReceiverAddress.Governate","RECV_Governate" },
                {"ReceiverAddress.Landmark","RECV_Landmark" },
                {"ReceiverAddress.PostalCode","RECV_PostalCode" },
                {"ReceiverAddress.RegionCity","RECV_RegionCity" },
                {"ReceiverAddress.Room","RECV_Room" },
                {"ReceiverAddress.Street","RECV_Street" }
            };
            while (reader.Read())
            {
                Document doc = reader.ReadDocument(propertyColumnMappings);
                doc.InvoiceLines = _invoiceLineDao.FindByDocumentId(doc.Id);
                IList<TaxTotal> taxTotals = new List<TaxTotal>();
                foreach (InvoiceLine line in doc.InvoiceLines)
                {
                    foreach (TaxableItem taxableItem in line.TaxableItems)
                    {
                        var found = (from tt in taxTotals
                                     where tt.TaxType == taxableItem.TaxType
                                     select tt).FirstOrDefault();
                        if (found == null)
                            taxTotals.Add(new TaxTotal() { TaxType = taxableItem.TaxType, Amount = taxableItem.Amount });
                        else
                            found.Amount += taxableItem.Amount;
                    }
                }
                doc.TaxTotals = taxTotals;
                documents.Add(doc);
            }
            reader.Close();
            _connection.Close();
            return documents;
        }
        public IList<Document> Find(Predicate<Document> predicate)
        {
            throw new NotImplementedException();
        }
        public void SaveOrUpdateDocument(Document document)
        {

            DbCommand command = _connection.CreateCommand("[dbo].[SaveOrUpdateDocument]", CommandType.StoredProcedure);
            command.Parameters.Add(command.CreateParameter(parameterName: "@OldId", parameterValue:document.Id,dbType:DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DocumentType", parameterValue: document.DocumentType, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DocumentTypeVersion", parameterValue: document.DocumentTypeVersion, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DateTimeIssued", parameterValue: document.DateTimeIssued, dbType: DbType.DateTime));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TaxpayerActivityCode", parameterValue: document.TaxpayerActivityCode, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@InternalId", parameterValue: document.InternalId, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@PurchaseOrderReference", parameterValue: document.PurchaseOrderReference, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@PurchaseOrderDescription", parameterValue: document.PurchaseOrderDescription, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@SalesOrderReference", parameterValue: document.SalesOrderReference, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@SalesOrderDescription", parameterValue: document.SalesOrderDescription, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@ProformaInvoiceNumber", parameterValue: document.ProformaInvoiceNumber, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@BankName", parameterValue: document.Payment.BankName, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@BankAddress", parameterValue: document.Payment.BankAddress, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@BankAccountNo", parameterValue: document.Payment.BankAccountNo, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@BankAccountIBAN", parameterValue: document.Payment.BankAccountIBAN, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@SwiftCode", parameterValue: document.Payment.SwiftCode, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@PaymentTerms", parameterValue: document.Payment.Terms, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Approach", parameterValue: document.Delivery.Approach, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Packaging", parameterValue: document.Delivery.Packaging, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DateValidity", parameterValue: Convert.ToDateTime(document.Delivery.DateValidity), dbType: DbType.DateTime));
            command.Parameters.Add(command.CreateParameter(parameterName: "@ExportPort", parameterValue: document.Delivery.ExportPort, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@CountryOfOrigin", parameterValue: document.Delivery.CountryOfOrigin, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@GrossWeight", parameterValue: document.Delivery.GrossWeight, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@NetWeight", parameterValue: document.Delivery.NetWeight, dbType: DbType.Decimal));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DeliveryTerms", parameterValue: document.Delivery.Terms, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TotalSalesAmount", parameterValue: document.TotalSalesAmount, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TotalDiscountAmount", parameterValue: document.TotalDiscountAmount, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@NetAmount", parameterValue: document.NetAmount, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@ExtraDiscountAmount", parameterValue: document.ExtraDiscountAmount, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TotalAmount", parameterValue: document.TotalAmount, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TotalItemsDiscountAmount", parameterValue: document.TotalItemsDiscountAmount, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@TaxpayerId", parameterValue: document.Issuer.Id, dbType: DbType.String));
            command.Parameters.Add(command.CreateParameter(parameterName: "@OldVerCol", parameterValue: document.Version, dbType: DbType.Binary));
            command.Parameters["@OldVerCol"].Size = 8;
            command.Parameters.Add(command.CreateParameter(parameterName: "@NewVerCol",direction:ParameterDirection.Output,dbType:DbType.Binary));
            command.Parameters["@NewVerCol"].Size = 8;
            command.Parameters.Add(command.CreateParameter(parameterName: "@NewId", direction: ParameterDirection.Output, dbType: DbType.Int32));
            command.Connection = _connection;
            document.Receiver.InternalId = _receiverDao.FindReceiverId(document.Receiver.Name);
            if (document.Receiver.InternalId == null)
                _receiverDao.Insert(document.Receiver);
            command.Parameters.Add(command.CreateParameter(parameterName: "@CustomerId", parameterValue: document.Receiver.InternalId, dbType: DbType.Int32));
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbTransaction transaction = null;
            try
            {
                transaction = _connection.BeginTransaction();
                command.Transaction = transaction;
                if (command.ExecuteNonQuery() > 0)
                {
                    document.Version = command.Parameters["@NewVerCol"].Value as byte[];
                    document.Id = command.Parameters["@NewId"].Value as int?;
                    foreach (InvoiceLine line in document.InvoiceLines)
                    {
                        line.DocumentId = document.Id;
                        _invoiceLineDao.SaveOrUpdate(line, transaction);
                    }
                    transaction.Commit();
                }
                
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
        public void SaveOrUpdateDocumentSubmission(DocumentSubmission submission)
        {
            DbCommand command = _connection.CreateCommand("[dbo].[SaveOrUpdateDocumentSubmission]", CommandType.StoredProcedure);
            command.Parameters.Add(command.CreateParameter(parameterName: "@APIEnvironmentId",parameterValue:submission.APIEnvironment.Id,dbType:DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@DocumentId", parameterValue: submission.Document.Id, dbType: DbType.Int32));
            command.Parameters.Add(command.CreateParameter(parameterName: "@UUID", parameterValue: submission.UUID));
            command.Parameters.Add(command.CreateParameter(parameterName: "@SubmissionUUID", parameterValue: submission.SubmissionUUID));
            command.Parameters.Add(command.CreateParameter(parameterName: "@SubmissionDate", parameterValue: submission.SubmissionDate,dbType:DbType.DateTime));
            command.Parameters.Add(command.CreateParameter(parameterName: "@Status", parameterValue: submission.Status));
            command.Parameters.Add(command.CreateParameter(parameterName: "@OldVerCol", parameterValue: submission.Version,dbType:DbType.Binary));
            command.Parameters["@OldVerCol"].Size = 8;
            command.Parameters.Add(command.CreateParameter(parameterName: "@NewVerCol",direction:ParameterDirection.Output,dbType:DbType.Binary));
            command.Parameters["@NewVerCol"].Size = 8;
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                if (command.ExecuteNonQuery() == 1)
                {
                    submission.Version = command.Parameters["@NewVerCol"].Value as byte[];
                }
                
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }
        //To be Revisited
        private void FixTableTax(Document document)
        {
            var toBeDeletedLines = (from line in document.InvoiceLines where line.InternalCode.StartsWith("VAT-") select line).ToList();
            foreach(var toBeDeletedLine in toBeDeletedLines)
            {
                var matchLine = (from line in document.InvoiceLines where line.InternalCode.Contains(toBeDeletedLine.InternalCode.Substring(4)) && !line.InternalCode.StartsWith("WR") select line).FirstOrDefault();
                if (matchLine != null)
                {
                    foreach(TaxableItem taxableItem in toBeDeletedLine.TaxableItems)
                    {
                        var foundTaxableItem = (from tt in matchLine.TaxableItems where tt.TaxType == taxableItem.TaxType && tt.SubType == taxableItem.SubType && tt.Rate == taxableItem.Rate select tt).FirstOrDefault();
                        if (foundTaxableItem == null)
                            matchLine.TaxableItems.Add(taxableItem);
                        else
                            foundTaxableItem.Amount += taxableItem.Amount;
                    }
                    matchLine.SalesTotal += toBeDeletedLine.SalesTotal;
                    matchLine.Total += toBeDeletedLine.Total;
                    matchLine.NetTotal += toBeDeletedLine.NetTotal;
                }
                document.InvoiceLines.Remove(toBeDeletedLine);
            }
        }
        public IList<Document> GetNewDataFromOracle(Issuer issuer,APIEnvironment environment)
        {
            try
            {
                DbCommand selectCommand = _connection.CreateCommand("[dbo].[FindNewDocumentsInOracle]", CommandType.StoredProcedure);
                selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@IssuerId", parameterValue: issuer.Id));
                selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@APIEnvId", parameterValue: environment.Id, dbType: DbType.Int32));
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
                selectCommand.CommandTimeout = 0;
                adapter.SelectCommand = selectCommand;
                DataSet dataSet = new DataSet("DocumentsNew");
                adapter.Fill(dataSet);
                IList<Document> documents = new List<Document>();
                var docHeaders = from doc in dataSet.Tables["table"].AsEnumerable() select doc;
                foreach (var docHeader in docHeaders)
                {
                    var docTaxTotals = new List<TaxTotal>();
                    Document doc = new Document();
                    doc.Id = null;
                    doc.DateTimeIssued = docHeader.Field<DateTime>("DOC_DATETIMEISS");
                    doc.DocumentType = docHeader.Field<string>("DOC_TYPE");
                    doc.DocumentTypeVersion = docHeader.Field<string>("DOC_TYPVER");
                    doc.InternalId = docHeader.Field<string>("DOC_INTERNALID");
                    doc.ProformaInvoiceNumber = docHeader.Field<string>("PROFORMAINVOICENUMBER");
                    doc.PurchaseOrderDescription = docHeader.Field<string>("DOC_PODESC");
                    doc.PurchaseOrderReference = docHeader.Field<string>("DOC_POREF");
                    doc.SalesOrderReference = docHeader.Field<string>("DOC_SOREF");
                    doc.SalesOrderDescription = docHeader.Field<string>("DOC_SODESC");
                    doc.TaxpayerActivityCode = docHeader.Field<string>("DOC_TAXPAYERACT");
                    doc.TotalAmount = Convert.ToDouble(docHeader.Field<decimal?>("TOTAMT")??0);
                    doc.TotalSalesAmount = Convert.ToDouble(docHeader.Field<decimal>("TOTALSALES"));
                    doc.TotalDiscountAmount = Convert.ToDouble(docHeader.Field<decimal>("TOTALDISCAMT"));
                    doc.NetAmount = Convert.ToDouble(docHeader.Field<decimal>("NETAMT"));
                    doc.TotalItemsDiscountAmount = Convert.ToDouble(docHeader.Field<decimal>("TOT_ITEMSDISCAMT"));
                    doc.ExtraDiscountAmount = Convert.ToDouble(docHeader.Field<decimal>("EXTRADISC"));
                    doc.Delivery = new Delivery()
                    {
                        Approach = docHeader.Field<string>("DEL_APPROACH"),
                        CountryOfOrigin = docHeader.Field<string>("DEL_COUNTRY"),
                        DateValidity = docHeader.Field<DateTime?>("DEL_DTVALID")?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ExportPort = docHeader.Field<string>("DEL_EXP"),
                        GrossWeight = Convert.ToInt32(docHeader.Field<decimal?>("DEL_GROSSWGHT")??0),
                        NetWeight = Convert.ToInt32(docHeader.Field<decimal?>("DEL_NETWGHT")??0),
                        Packaging = docHeader.Field<string>("DEL_PACK"),
                        Terms = docHeader.Field<string>("DEL_TERMS")
                    };
                    doc.Payment = new Payment()
                    {
                        SwiftCode = "",
                        Terms = docHeader.Field<string>("PAY_TERMS"),
                        BankAccountIBAN = "",
                        BankAccountNo = "",
                        BankAddress = "",
                        BankName = ""
                    };
                    doc.Issuer = new Issuer()
                    {
                        Id = docHeader.Field<string>("ISS_ID"),
                        Name = docHeader.Field<string>("ISS_NAME"),
                        Type = docHeader.Field<string>("ISS_TYPE"),
                        Address = new IssuerAddress()
                        {
                            BranchId = docHeader.Field<string>("ISS_BRANCHID"),
                            BuildingNumber = docHeader.Field<string>("ISS_BLDGNO"),
                            AdditionalInformation = docHeader.Field<string>("ISS_ADDINFO"),
                            Floor = docHeader.Field<string>("ISS_FLOOR"),
                            Country = docHeader.Field<string>("ISS_COUNTRY"),
                            Governate = docHeader.Field<string>("ISS_GOVERNATE"),
                            Landmark = docHeader.Field<string>("ISS_LANDMARK"),
                            PostalCode = docHeader.Field<string>("ISS_POSTAL"),
                            RegionCity = docHeader.Field<string>("ISS_REGIONCITY"),
                            Room = docHeader.Field<string>("ISS_ROOM"),
                            Street = docHeader.Field<string>("ISS_STREET")
                        }
                    };
                    doc.Receiver = new Receiver()
                    {
                        Id = docHeader.Field<string>("RECV_ID"),
                        Type = docHeader.Field<string>("RECV_TYPE").ToReceiverType(),
                        Name = docHeader.Field<string>("RECV_NAME"),
                        Address = new ReceiverAddress()
                        {
                            AdditionalInformation = docHeader.Field<string>("RECV_ADDINFO"),
                            BuildingNumber = docHeader.Field<string>("RECV_BLDGNO"),
                            Country = docHeader.Field<string>("RECV_COUNTRY"),
                            Floor = docHeader.Field<string>("RECV_FLOOR"),
                            Governate = docHeader.Field<string>("RECV_GOVERNATE"),
                            Landmark = docHeader.Field<string>("RECV_LANDMARK"),
                            PostalCode = docHeader.Field<string>("RECV_POSTAL"),
                            RegionCity = docHeader.Field<string>("RECV_REGIONCITY"),
                            Room = docHeader.Field<string>("RECV_ROOM"),
                            Street = docHeader.Field<string>("RECV_STREET")
                        },
                        InternalId = _receiverDao?.FindReceiverId(docHeader.Field<string>("RECV_NAME"))
                    };
                    //Add Receiver if not exists
                    if (doc.Receiver.InternalId == null)
                    {
                        _receiverDao.Insert(doc.Receiver);
                    }
                    var docLines = from line in dataSet.Tables["table1"].AsEnumerable() where line.Field<string>("SOPNUMBER") == doc.InternalId select line;
                    foreach (var docLine in docLines)
                    {
                        InvoiceLine invoiceLine = new InvoiceLine()
                        {
                            Description = docLine.Field<string>("LIN_DESC"),
                            InternalCode = docLine.Field<string>("LIN_INTERNALCODE"),
                            ItemCode = docLine.Field<string>("LIN_ITEMCODE"),
                            ItemType = docLine.Field<string>("LIN_ITEMTYPE"),
                            Quantity = Convert.ToDouble(docLine.Field<decimal>("LIN_QTY")),
                            ItemsDiscount = Convert.ToDouble(docLine.Field<decimal>("ITM_DISC")),
                            ValueDifference = Convert.ToDouble(docLine.Field<decimal>("VAL_DIFF")),
                            NetTotal = Convert.ToDouble(docLine.Field<decimal>("NET_TOT")),
                            UnitType = docLine.Field<string>("LIN_UNTTYP"),
                            SalesTotal = Convert.ToDouble(docLine.Field<decimal>("SAL_TOT")),
                            TotalTaxableFees = Convert.ToDouble(docLine.Field<decimal>("TAXABLE_FEE")),
                            Discount = new Discount()
                            {
                                Amount = Convert.ToDouble(docLine.Field<decimal>("DISC_AMT")),
                                Rate = Convert.ToDouble(Math.Round(docLine.Field<decimal>("DISC_RATE")))
                            },
                            Total = Convert.ToDouble(docLine.Field<decimal>("TOTAL")),
                            UnitValue = new Value()
                            {
                                AmountEGP = Convert.ToDouble(docLine.Field<decimal>("LIN_AMTEGP")),
                                AmountSold = Convert.ToDouble(docLine.Field<decimal>("LIN_AMTSOLD")),
                                CurrencyExchangeRate = Convert.ToDouble(docLine.Field<decimal>("LIN_CURREXCH")),
                                CurrencySold = docLine.Field<string>("LIN_CURSOLD")
                            },
                        };
                        var linetaxDetails = from det in dataSet.Tables["table2"].AsEnumerable()
                                             where det.Field<string>("SOPNUMBER") == docLine.Field<string>("SOPNUMBER") && det.Field<string>("LINE_ITEM_SEQ") == docLine.Field<string>("LINE_ITEM_SEQ")
                                             select det;
                        foreach (var detail in linetaxDetails)
                        {
                            TaxableItem taxableItem = new TaxableItem()
                            {
                                Amount = Convert.ToDouble(detail.Field<decimal>("AMT")),
                                Rate = Convert.ToDouble(detail.Field<decimal>("TAXPERCENT")),
                                TaxType = detail.Field<string>("TAXTYPE"),
                                SubType = detail.Field<string>("TAXSUBTYPE")
                            };
                            var found = docTaxTotals.Find(tt => { return tt.TaxType == taxableItem.TaxType; });
                            if (found != null)
                                found.Amount += taxableItem.Amount;
                            else
                                docTaxTotals.Add(new TaxTotal() { TaxType = taxableItem.TaxType, Amount = taxableItem.Amount });
                            invoiceLine.TaxableItems.Add(taxableItem);
                        }
                        doc.InvoiceLines.Add(invoiceLine);
                    }
                    //FixTableTax(doc);
                    foreach (TaxTotal taxTotal in docTaxTotals)
                    {
                        taxTotal.Amount = Math.Round(taxTotal.Amount, 5);
                    }
                    doc.TaxTotals = docTaxTotals;
                    documents.Add(doc);
                }
                return documents;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
            }
        }
        public IList<Document> FindDocumentInOracleByInternalId(string internalId, Issuer issuer)
        {
            IList<Document> documents = new List<Document>();
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetDocumentFromOracleByInternalId]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@DOC_InternalId", parameterValue: internalId, dbType: DbType.String));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@ISS_ID", parameterValue: issuer.Id, dbType: DbType.String));
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataAdapter adapter = DbProviderFactories.GetFactory(_connection).CreateDataAdapter();
            selectCommand.CommandTimeout = 0;
            adapter.SelectCommand = selectCommand;
            DataSet dataSet = new DataSet("DocumentsNew");
            adapter.Fill(dataSet);
            var docHeaders = from doc in dataSet.Tables["table"].AsEnumerable() select doc;
            foreach (var docHeader in docHeaders)
            {
                var docTaxTotals = new List<TaxTotal>();
                Document doc = new Document();
                doc.Id = null;
                doc.DateTimeIssued = docHeader.Field<DateTime>("DOC_DATETIMEISS");
                doc.DocumentType = docHeader.Field<string>("DOC_TYPE");
                doc.DocumentTypeVersion = docHeader.Field<string>("DOC_TYPVER");
                doc.InternalId = docHeader.Field<string>("DOC_INTERNALID");
                doc.ProformaInvoiceNumber = docHeader.Field<string>("PROFORMAINVOICENUMBER");
                doc.PurchaseOrderDescription = docHeader.Field<string>("DOC_PODESC");
                doc.PurchaseOrderReference = docHeader.Field<string>("DOC_POREF");
                doc.SalesOrderReference = docHeader.Field<string>("DOC_SOREF");
                doc.SalesOrderDescription = docHeader.Field<string>("DOC_SODESC");
                doc.TaxpayerActivityCode = docHeader.Field<string>("DOC_TAXPAYERACT");
                doc.TotalAmount = Convert.ToDouble(docHeader.Field<decimal>("TOTAMT"));
                doc.TotalSalesAmount = Convert.ToDouble(docHeader.Field<decimal>("TOTALSALES"));
                doc.TotalDiscountAmount = Convert.ToDouble(docHeader.Field<decimal>("TOTALDISCAMT"));
                doc.NetAmount = Convert.ToDouble(docHeader.Field<decimal>("NETAMT"));
                doc.TotalItemsDiscountAmount = Convert.ToDouble(docHeader.Field<decimal>("TOT_ITEMSDISCAMT"));
                doc.ExtraDiscountAmount = Convert.ToDouble(docHeader.Field<decimal>("EXTRADISC"));
                doc.Delivery = new Delivery()
                {
                    Approach = docHeader.Field<string>("DEL_APPROACH"),
                    CountryOfOrigin = docHeader.Field<string>("DEL_COUNTRY"),
                    DateValidity = docHeader.Field<DateTime?>("DEL_DTVALID")?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ExportPort = docHeader.Field<string>("DEL_EXP"),
                    GrossWeight = Convert.ToInt32(docHeader.Field<decimal?>("DEL_GROSSWGHT") ?? 0),
                    NetWeight = Convert.ToInt32(docHeader.Field<decimal?>("DEL_NETWGHT") ?? 0),
                    Packaging = docHeader.Field<string>("DEL_PACK"),
                    Terms = docHeader.Field<string>("DEL_TERMS")
                };
                doc.Payment = new Payment()
                {
                    SwiftCode = "",
                    Terms = docHeader.Field<string>("PAY_TERMS"),
                    BankAccountIBAN = "",
                    BankAccountNo = "",
                    BankAddress = "",
                    BankName = ""
                };
                doc.Issuer = new Issuer()
                {
                    Id = docHeader.Field<string>("ISS_ID"),
                    Name = docHeader.Field<string>("ISS_NAME"),
                    Type = docHeader.Field<string>("ISS_TYPE"),
                    Address = new IssuerAddress()
                    {
                        BranchId = docHeader.Field<string>("ISS_BRANCHID"),
                        BuildingNumber = docHeader.Field<string>("ISS_BLDGNO"),
                        AdditionalInformation = docHeader.Field<string>("ISS_ADDINFO"),
                        Floor = docHeader.Field<string>("ISS_FLOOR"),
                        Country = docHeader.Field<string>("ISS_COUNTRY"),
                        Governate = docHeader.Field<string>("ISS_GOVERNATE"),
                        Landmark = docHeader.Field<string>("ISS_LANDMARK"),
                        PostalCode = docHeader.Field<string>("ISS_POSTAL"),
                        RegionCity = docHeader.Field<string>("ISS_REGIONCITY"),
                        Room = docHeader.Field<string>("ISS_ROOM"),
                        Street = docHeader.Field<string>("ISS_STREET")
                    }
                };
                doc.Receiver = new Receiver()
                {
                    Id = docHeader.Field<string>("RECV_ID"),
                    Type = docHeader.Field<string>("RECV_TYPE").ToReceiverType(),
                    Name = docHeader.Field<string>("RECV_NAME"),
                    Address = new ReceiverAddress()
                    {
                        AdditionalInformation = docHeader.Field<string>("RECV_ADDINFO"),
                        BuildingNumber = docHeader.Field<string>("RECV_BLDGNO"),
                        Country = docHeader.Field<string>("RECV_COUNTRY"),
                        Floor = docHeader.Field<string>("RECV_FLOOR"),
                        Governate = docHeader.Field<string>("RECV_GOVERNATE"),
                        Landmark = docHeader.Field<string>("RECV_LANDMARK"),
                        PostalCode = docHeader.Field<string>("RECV_POSTAL"),
                        RegionCity = docHeader.Field<string>("RECV_REGIONCITY"),
                        Room = docHeader.Field<string>("RECV_ROOM"),
                        Street = docHeader.Field<string>("RECV_STREET")
                    },
                    InternalId = _receiverDao?.FindReceiverId(docHeader.Field<string>("RECV_NAME"))
                };
                //Add Receiver if not exists
                if (doc.Receiver.InternalId == null)
                {
                    _receiverDao.Insert(doc.Receiver);
                }
                var docLines = from line in dataSet.Tables["table1"].AsEnumerable() where line.Field<string>("SOPNUMBER") == doc.InternalId select line;
                foreach (var docLine in docLines)
                {
                    InvoiceLine invoiceLine = new InvoiceLine()
                    {
                        Description = docLine.Field<string>("LIN_DESC"),
                        InternalCode = docLine.Field<string>("LIN_INTERNALCODE"),
                        ItemCode = docLine.Field<string>("LIN_ITEMCODE"),
                        ItemType = docLine.Field<string>("LIN_ITEMTYPE"),
                        Quantity = Convert.ToDouble(docLine.Field<decimal>("LIN_QTY")),
                        ItemsDiscount = Convert.ToDouble(docLine.Field<decimal>("ITM_DISC")),
                        ValueDifference = Convert.ToDouble(docLine.Field<decimal>("VAL_DIFF")),
                        NetTotal = Convert.ToDouble(docLine.Field<decimal>("NET_TOT")),
                        UnitType = docLine.Field<string>("LIN_UNTTYP"),
                        SalesTotal = Convert.ToDouble(docLine.Field<decimal>("SAL_TOT")),
                        TotalTaxableFees = Convert.ToDouble(docLine.Field<decimal>("TAXABLE_FEE")),
                        Discount = new Discount()
                        {
                            Amount = Convert.ToDouble(docLine.Field<decimal>("DISC_AMT")),
                            Rate = Convert.ToDouble(Math.Round(docLine.Field<decimal>("DISC_RATE")))
                        },
                        Total = Convert.ToDouble(docLine.Field<decimal>("TOTAL")),
                        UnitValue = new Value()
                        {
                            AmountEGP = Convert.ToDouble(docLine.Field<decimal>("LIN_AMTEGP")),
                            AmountSold = Convert.ToDouble(docLine.Field<decimal>("LIN_AMTSOLD")),
                            CurrencyExchangeRate = Convert.ToDouble(docLine.Field<decimal>("LIN_CURREXCH")),
                            CurrencySold = docLine.Field<string>("LIN_CURSOLD")
                        },
                    };
                    var linetaxDetails = from det in dataSet.Tables["table2"].AsEnumerable()
                                         where det.Field<string>("SOPNUMBER") == docLine.Field<string>("SOPNUMBER") && det.Field<string>("LINE_ITEM_SEQ") == docLine.Field<string>("LINE_ITEM_SEQ")
                                         select det;
                    foreach (var detail in linetaxDetails)
                    {
                        TaxableItem taxableItem = new TaxableItem()
                        {
                            Amount = Convert.ToDouble(detail.Field<decimal>("AMT")),
                            Rate = Convert.ToDouble(detail.Field<decimal>("TAXPERCENT")),
                            TaxType = detail.Field<string>("TAXTYPE"),
                            SubType = detail.Field<string>("TAXSUBTYPE")
                        };
                        var found = docTaxTotals.Find(tt => { return tt.TaxType == taxableItem.TaxType; });
                        if (found != null)
                            found.Amount += taxableItem.Amount;
                        else
                            docTaxTotals.Add(new TaxTotal() { TaxType = taxableItem.TaxType, Amount = taxableItem.Amount });
                        invoiceLine.TaxableItems.Add(taxableItem);
                    }
                    doc.InvoiceLines.Add(invoiceLine);
                }
                //FixTableTax(doc);
                foreach (TaxTotal taxTotal in docTaxTotals)
                {
                    taxTotal.Amount = Math.Round(taxTotal.Amount, 5);
                }
                doc.TaxTotals = docTaxTotals;
                documents.Add(doc);
            }
            return documents;
        }
        public IList<Document> GetUnsubmittedDocuments(Issuer issuer, APIEnvironment environment)
        {
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetUnsubmittedDocumentsFromOracle]", CommandType.StoredProcedure);
            IList<Document> documents = new List<Document>();
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@TaxpayerId", parameterValue: issuer.Id, dbType: DbType.String));
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@APIEnvId",parameterValue:environment.Id,dbType:DbType.Int32));
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>()
            {
                { "Document.DateTimeIssued","DateTimeIssued" },
                { "Document.DocumentType","DocumentType" },
                { "Document.DocumentTypeVersion","DocumentTypeVersion" },
                { "Document.ExtraDiscountAmount","ExtraDiscountAmount" },
                { "Document.InternalId","InternalId" },
                { "Document.Id","Id" },
                { "Document.NetAmount","NetAmount" },
                { "Document.ProformaInvoiceNumber","ProformaInvoiceNumber" },
                { "Document.PurchaseOrderDescription","PurchaseOrderDescription" },
                { "Document.PurchaseOrderReference","PurchaseOrderReference" },
                { "Document.SalesOrderDescription","SalesOrderDescription" },
                { "Document.SalesOrderReference","SalesOrderReference" },
                { "Document.TaxpayerActivityCode","TaxpayerActivityCode" },
                { "Document.TotalAmount","TotalAmount" },
                { "Document.TotalSalesAmount","TotalSalesAmount" },
                { "Document.TotalDiscountAmount","TotalDiscountAmount" },
                { "Document.Version","VerCol" },
                { "Document.TotalItemsDiscountAmount","TotalItemsDiscountAmount" },
                { "Issuer.Id","TaxpayerId" },
                { "Issuer.Name","ISS_Name" },
                { "Issuer.Type","ISS_Type" },
                { "Issuer.Version","ISS_VerCol" },
                { "IssuerAddress.AdditionalInformation","ISS_AdditionalInformation" },
                { "IssuerAddress.BranchId","ISS_BranchId" },
                { "IssuerAddress.BuildingNumber","ISS_BuildingNumber" },
                { "IssuerAddress.Country","ISS_Country" },
                { "IssuerAddress.Floor","ISS_Floor" },
                { "IssuerAddress.Governate","ISS_Governate" },
                { "IssuerAddress.Landmark","ISS_Landmark" },
                { "IssuerAddress.PostalCode","ISS_PostalCode" },
                { "IssuerAddress.RegionCity","ISS_RegionCity" },
                { "IssuerAddress.Room","ISS_Room" },
                { "IssuerAddress.Street","ISS_Street" },
                {"Delivery.Approach","Approach" },
                { "Delivery.CountryOfOrigin","CountryOfOrigin" },
                { "Delivery.DateValidity","DateValidity" },
                { "Delivery.ExportPort","ExportPort" },
                { "Delivery.GrossWeight","GrossWeight" },
                { "Delivery.NetWeight","NetWeight" },
                { "Delivery.Packaging","Packaging" },
                { "Delivery.Terms","DeliveryTerms" },
                {"Payment.BankAccountIBAN","BankAccountIBAN" },
                {"Payment.BankAccountNo","BankAccountNo" },
                {"Payment.BankAddress","BankAddress" },
                {"Payment.BankName","BankName" },
                {"Payment.SwiftCode","SwiftCode" },
                {"Payment.Terms","PaymentTerms" },
                {"Receiver.InternalId","CustomerId" },
                {"Receiver.Id","RECV_Id" },
                {"Receiver.Name","RECV_Name" },
                {"Receiver.Type","RECV_Type" },
                {"Receiver.Version","RECV_VerCol" },
                {"ReceiverAddress.AdditionalInformation","RECV_AdditionalInformation" },
                {"ReceiverAddress.BuildingNumber","RECV_BuildingNumber" },
                {"ReceiverAddress.Country","RECV_Country" },
                {"ReceiverAddress.Floor","RECV_Floor" },
                {"ReceiverAddress.Governate","RECV_Governate" },
                {"ReceiverAddress.Landmark","RECV_Landmark" },
                {"ReceiverAddress.PostalCode","RECV_PostalCode" },
                {"ReceiverAddress.RegionCity","RECV_RegionCity" },
                {"ReceiverAddress.Room","RECV_Room" },
                {"ReceiverAddress.Street","RECV_Street" }
            };
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Document doc = reader.ReadDocument(propertyColumnMappings);
                        doc.InvoiceLines = _invoiceLineDao.FindByDocumentId(doc.Id);
                        IList<TaxTotal> taxTotals = new List<TaxTotal>();
                        foreach (InvoiceLine line in doc.InvoiceLines)
                        {
                            foreach (TaxableItem taxableItem in line.TaxableItems)
                            {
                                var found = (from tt in taxTotals
                                             where tt.TaxType == taxableItem.TaxType
                                             select tt).FirstOrDefault();
                                if (found == null)
                                    taxTotals.Add(new TaxTotal() { TaxType = taxableItem.TaxType, Amount = taxableItem.Amount });
                                else
                                    found.Amount += taxableItem.Amount;
                            }
                        }
                        doc.TaxTotals = taxTotals;
                        documents.Add(doc);
                    }
                }
                return documents;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }
        public void Insert(Document document)
        {
            if (document != null)
            {
                DbCommand insertCommand = CreateInsertCommand(document);
                if(_connection.State != ConnectionState.Open)
                        _connection.Open();
                using (DbTransaction trans = _connection.BeginTransaction())
                {
                    insertCommand.Transaction = trans;
                    insertCommand.ExecuteNonQuery();
                    document.Id = insertCommand.Parameters["@DocumentId"].Value as int?;
                    document.Version = insertCommand.Parameters["@VerCol"].Value as byte[];
                    foreach (InvoiceLine line in document.InvoiceLines)
                        _invoiceLineDao.Insert(line,trans);
                    trans.Commit();
                }
                 _connection.Close();
            }
        }
        public Document Find(int? id)
        {
            Dictionary<string, string> propertyColumnMappings = new Dictionary<string, string>() 
            {
                { "Document.Id" , "Id" },
                { "Document.DocumentType","DocumentType" },
                { "Document.DocumentTypeVersion","DocumentTypeVersion" },
                { "Document.DateTimeIssued","DateTimeIssued" },
                { "Receiver.InternalId","CustomerId"},
                { "Receiver.Name","RECV_Name" },
                { "ReceiverAddress.AdditionalInformation","Recv_additionalInformation" },
                { "ReceiverAddress.BuildingNumber","RECV_BuildingNumber" },
                { "ReceiverAddress.Country","RECV_Country" },
                { "ReceiverAddress.Floor","RECV_Floor" },
                { "ReceiverAddress.Governate","RECV_Governate" },
                { "Receiver.Id","RECV_Id" },
                { "ReceiverAddress.Landmark","RECV_Landmark" },
                { "ReceiverAddress.PostalCode","RECV_PostalCode" },
                { "ReceiverAddress.RegionCity","RECV_RegionCity" },
                { "ReceiverAddress.Room","RECV_Room" },
                { "ReceiverAddress.Street","RECV_Street" },
                { "Receiver.Type","RECV_type" },
                { "Receiver.Version","RECV_VerCol" },
                { "Payment.BankAccountIBAN" , "BankAccountIBAN" },
                { "Payment.BankAccountNo","BankAccountNo"},
                { "Payment.BankAddress","BankAddress"},
                { "Payment.BankName" , "BankName" },
                { "Payment.SwiftCode","SwiftCode" },
                { "Delivery.CountryOfOrigin" , "CountryOfOrigin" },
                { "Delivery.DateValidity","DateValidity" },
                { "Delivery.Approach" , "Approach" },
                { "Delivery.Terms","DeliveryTerms" },
                { "Delivery.ExportPort","ExportPort" },
                { "Delivery.GrossWeight","GrossWeight" },
                { "Delivery.NetWeight","NetWeight" },
                { "Delivery.Packaging","Packaging" },
                { "Delivery.Terms","PaymentTerms" },
                { "Issuer.Id","TaxpayerId" },
                { "IssuerAddress.AdditionalInformation","ISS_AdditionalInformation" },
                { "IssuerAddress.BranchId","ISS_BranchId" },
                { "IssuerAddress.BuildingNumber","ISS_BuildingNumber" },
                { "IssuerAddress.Country","ISS_Country" },
                { "IssuerAddress.Floor","ISS_Floor" },
                { "IssuerAddress.Governate","ISS_Governate" },
                { "IssuerAddress.Landmark","ISS_Landmark" },
                { "Issuer.Name","ISS_Name" },
                { "IssuerAddress.PostalCode","ISS_PostalCode" },
                { "IssuerAddress.RegionCity","ISS_RegionCity" },
                { "IssuerAddress.Room","ISS_Room" },
                { "IssuerAddress.Street","ISS_Street" },
                { "Issuer.Type","ISS_Type" },
                { "Issuer.Version","ISS_VerCol" },
                { "Document.ProformaInvoiceNumber","ProformaInvoiceNumber" },
                { "Document.PurchaseOrderDescription","PurchaseOrderDescription" },
                { "Document.PurchaseOrderReference","PurchaseOrderReference" },
                { "Document.SalesOrderDescription","SalesOrderDescription" },
                { "Document.SalesOrderReference","SalesOrderReference" },
                { "Document.TaxpayerActivityCode","TaxpayerActivityCode" },
                { "Document.TotalAmount","TotalAmount" },
                { "Document.TotalDiscountAmount","TotalDiscountAmount" },
                { "Document.TotalItemsDiscountAmount","TotalItemsDiscountAmount" },
                { "Document.TotalSalesAmount","TotalSalesAmount" },
                { "Document.InternalId","InternalId" },
                { "Document.NetAmount","NetAmount" },
                { "Document.Version","VerCol" }
            };
            DbCommand selectCommand = _connection.CreateCommand("[dbo].[GetDocumentById]", CommandType.StoredProcedure);
            selectCommand.Parameters.Add(selectCommand.CreateParameter(parameterName: "@Id", parameterValue: id, dbType: DbType.Int32));
            selectCommand.Connection = _connection;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            DbDataReader reader = selectCommand.ExecuteReader();
            Document doc = null;
            while (reader.Read())
            {
                doc = reader.ReadDocument(propertyColumnMappings);
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    doc.InvoiceLines.Add(reader.ReadInvoiceLine(propertyColumnMappings));
                }
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    TaxableItem taxableItem = reader.ReadTaxableItem(propertyColumnMappings);
                    (from line in doc.InvoiceLines where line.Id == taxableItem.InvoiceLineId select line).First().TaxableItems.Add(taxableItem);
                }
            }
            reader.Close();
            _connection.Close();
            return doc;
        }
        private DbCommand CreateInsertCommand(Document document)
        {
            DbCommand insertCommand = _connection.CreateCommand("[dbo].[InsertDocument]",CommandType.StoredProcedure);
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DocumentType", parameterValue: document.DocumentType));
            insertCommand.Parameters["@DocumentType"].Size = 20;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DocumentTypeVersion", parameterValue: document.DocumentTypeVersion));
            insertCommand.Parameters["@DocumentTypeVersion"].Size = 100;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DateTimeIssued", parameterValue: document.DateTimeIssued,dbType:DbType.DateTime));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TaxpayerActivityCode", parameterValue: document.TaxpayerActivityCode));
            insertCommand.Parameters["@TaxpayerActivityCode"].Size = 10;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@InternalId", parameterValue: document.InternalId));
            insertCommand.Parameters["@InternalId"].Size = 50;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@PurchaseOrderReference", parameterValue: document.PurchaseOrderReference));
            insertCommand.Parameters["@PurchaseOrderReference"].Size = 100;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@PurchaseOrderDescription", parameterValue: document.PurchaseOrderDescription));
            insertCommand.Parameters["@PurchaseOrderDescription"].Size = 500;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@SalesOrderReference", parameterValue: document.SalesOrderReference));
            insertCommand.Parameters["@SalesOrderReference"].Size = 100;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@SalesOrderDescription", parameterValue: document.SalesOrderDescription));
            insertCommand.Parameters["@SalesOrderDescription"].Size = 500;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ProformaInvoiceNumber", parameterValue: document.ProformaInvoiceNumber));
            insertCommand.Parameters["@ProformaInvoiceNumber"].Size = 50;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@BankName", parameterValue: document?.Payment.BankName));
            insertCommand.Parameters["@BankName"].Size = 100;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@BankAddress", parameterValue: document?.Payment.BankAddress));
            insertCommand.Parameters["@BankAddress"].Size = 500;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@BankAccountNo", parameterValue: document?.Payment.BankAccountNo));
            insertCommand.Parameters["@BankAccountNo"].Size = 50;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@BankAccountIBAN", parameterValue: document?.Payment.BankAccountIBAN));
            insertCommand.Parameters["@BankAccountIBAN"].Size = 50;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@SwiftCode", parameterValue: document?.Payment.SwiftCode));
            insertCommand.Parameters["@SwiftCode"].Size = 500;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@PaymentTerms", parameterValue: document?.Payment.Terms));
            insertCommand.Parameters["@PaymentTerms"].Size = 500;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Approach", parameterValue: document?.Delivery.Approach));
            insertCommand.Parameters["@Approach"].Size = 100;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@Packaging",parameterValue:document?.Delivery.Packaging));
            insertCommand.Parameters["@Packaging"].Size = 100;
            if(string.IsNullOrEmpty(document?.Delivery?.DateValidity))
                insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DateValidity", parameterValue: DBNull.Value, dbType:DbType.DateTime));
             else if(DateTime.TryParse(document?.Delivery?.DateValidity, out DateTime result))
                 insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DateValidity", parameterValue: result, dbType: DbType.DateTime));
            else
                insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DateValidity", parameterValue: DBNull.Value, dbType: DbType.DateTime));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@ExportPort", parameterValue: document?.Delivery?.ExportPort));
            insertCommand.Parameters["@ExportPort"].Size = 100;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@CountryOfOrigin", parameterValue: document?.Delivery.CountryOfOrigin));
            insertCommand.Parameters["@CountryOfOrigin"].Size = 100;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@GrossWeight", parameterValue: document?.Delivery.GrossWeight,dbType:DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@NetWeight", parameterValue: document?.Delivery.NetWeight, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DeliveryTerms", parameterValue: document?.Delivery.Terms, dbType: DbType.String));
            insertCommand.Parameters["@DeliveryTerms"].Size = 500;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TotalSalesAmount", parameterValue: document.TotalSalesAmount, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TotalDiscountAmount", parameterValue: document.TotalDiscountAmount, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@NetAmount", parameterValue: document.NetAmount, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TotalAmount", parameterValue: document.TotalAmount, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TotalItemsDiscountAmount", parameterValue: document.TotalItemsDiscountAmount, dbType: DbType.Decimal));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@TaxpayerId", parameterValue: document.Issuer.Id, dbType: DbType.String));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@CustomerId", parameterValue: document.Receiver.InternalId, dbType: DbType.Int32));
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@DocumentId", dbType: DbType.Int32,direction:ParameterDirection.Output));
            insertCommand.Parameters["@DocumentId"].SourceColumnNullMapping = true;
            insertCommand.Parameters.Add(insertCommand.CreateParameter(parameterName: "@VerCol", dbType: DbType.Binary, direction: ParameterDirection.Output));
            insertCommand.Parameters["@VerCol"].SourceColumnNullMapping = true;
            insertCommand.Parameters["@VerCol"].Size = 8;
            return insertCommand;
        }
    }
}
