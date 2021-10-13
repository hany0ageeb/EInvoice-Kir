using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EInvoice.Model;
using Newtonsoft.Json;

namespace EInvoice.DAL.EInvoiceAPI
{
    public interface IEInvoiceAPIProxy
    {
        DocumentTypeVersion GetDocumentTypeVersion(int id,int vid);
        SubmissionResult SubmitDocuments(string jsonText);
        DocumentExtended GetDocument(string uuid);
        RecentDocumentsResult GetRecentDocuments(int pageNo,int pageSize);
        string RequestDocumentPackage(DocumentPackageRequest request);
        byte[] GetDocumentPackage(string requestId);
        DocumentPackageRequestResult GetPackageRequests(int pageNo, int pageSize);
        byte[] GetDocumentPrintOut(string uuid);
        APIEnvironment Environment { get; }
    }
    
}
