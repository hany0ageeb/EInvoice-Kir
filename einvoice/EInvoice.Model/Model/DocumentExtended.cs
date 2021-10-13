using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class DocumentExtended
    {
        [JsonIgnore]
        public string InvoiceNumber { get; set; }
        public string uuid { get; set; }
        public string submissionUUID { get; set; }
        public string longId { get; set; }
        public string internalId { get; set; }
        public string typeName { get; set; }
        public string typeVersionName { get; set; }
        public string issuerId { get; set; }
        public string issuerName { get; set; }
        public string receiverId { get; set; }
        public string receiverName { get; set; }
        public DateTime dateTimeIssued { get; set; }
        public DateTime? dateTimeReceived { get; set; }
        public decimal totalSales { get; set; }
        public decimal totalDiscount { get; set; }
        public decimal netAmount { get; set; }
        public decimal total { get; set; }
        public string status { get; set; }
        public string transformationStatus { get; set; }
        public int maxPercision { get; set; }
        public IList<InvoiceLineItemCode> invoiceLineItemCodes = new List<InvoiceLineItemCode>();
        public DocumentValidationResult validationResults { get; set; }
        public string document { get; set; }

        public Document taxAuthorityDocument { get; set; }
    }
}
