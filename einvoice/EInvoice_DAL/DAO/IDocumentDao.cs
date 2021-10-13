using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EInvoice.Model;
using System;

namespace EInvoice.DAL.DAO
{
    public interface IDocumentDao : IEntityDao<Document>
    {
        IList<Document> GetNewDataFromOracle(Issuer issuer,APIEnvironment environment);
        IList<Document> GetUnsubmittedDocuments(Issuer issuer, APIEnvironment environment);
        void SaveOrUpdateDocument(Document document);
        void SaveOrUpdateDocumentSubmission(DocumentSubmission submission);
        void InsertDocumentSubmission(DocumentSubmission submission);
        IList<Document> Find();
        IList<Document> FindByInternalId(string internalId);
        IList<Document> Find(Predicate<Document> predicate);
        bool IsDocumentSubmissionExists(int? docId, int? EnvId);
        IList<InvoiceSummaryView> GetInvoiceSummary(int APIEnvironmentId, string IssuerId, string ReceiverName = "%", DateTime? issueDateFrom = null, DateTime? issueDateTo = null);
        IList<DocumentSubmission> FindDocumentSubmissionsByState(Issuer issuer,APIEnvironment env,string state);
        IList<DocumentExtended> FindDocumentSubmissions(
            DateTime? issueDateFrom,
            DateTime? issueDateTo,
            DateTime? submissionDateFrom,
            DateTime? submissionDateTo,
            Issuer issuer,
            int APIEnvironmentId,
            Receiver receiver,
            string proformaInvoiceNumber,
            string status
        );
    }
    
}
