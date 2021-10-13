using System.Collections.Generic;

namespace EInvoice.Model
{
    public class RecentDocumentsResult
    {
        public IList<DocumentSummary> result = new List<DocumentSummary>();
        public Metadata metadata { get; set; }
    }
}
