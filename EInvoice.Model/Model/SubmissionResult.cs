using System.Collections.Generic;

namespace EInvoice.Model
{
    public class SubmissionResult
    {
        public string submissionId { get; set; }
        public IList<DocumentAccepted> acceptedDocuments { get; set; }
        public IList<DocumentRejected> rejectedDocuments { get; set; }
    }
}
