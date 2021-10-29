using System;

namespace EInvoice.Model
{
    public class DocumentPackageInformation
    {
        public string packageId { get; set; }
        public DateTime submissionDate { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public int format { get; set; }
        public int requestorTypeId { get; set; }
        public string requestorTaxpayerRIN { get; set; }
        public string requestorTaxpayerName { get; set; }
        public bool isExpired { get; set; } = false;
        public QueryParameters queryParameters { get; set; }
    }
}
