using System.Collections.Generic;

namespace EInvoice.Model
{
    public class DocumentPackageRequestResult
    {
        public IList<DocumentPackageInformation> result { get; set; }
        public Metadata metadata { get; set; }
    }
}
