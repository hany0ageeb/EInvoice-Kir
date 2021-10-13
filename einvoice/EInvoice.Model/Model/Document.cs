using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class Document
    {
        private int? _id = null;
        [JsonIgnore]
        public int? Id 
        { 
            get => _id; 
            set 
            {
                _id = value;
                foreach (InvoiceLine invoiceLine in InvoiceLines)
                    invoiceLine.DocumentId = _id;
            } 
        } 
        [JsonProperty("issuer")]
        public Issuer Issuer { get; set; }
        [JsonProperty("receiver")]
        public Receiver Receiver { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(20,MinimumLength = 1)]
        [JsonProperty("documentType")]
        public string DocumentType { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(100,MinimumLength = 1 )]
        [JsonProperty("documentTypeVersion")]
        public string DocumentTypeVersion { get; set; }
        [Required]
        [JsonProperty("dateTimeIssued")]
        public DateTime DateTimeIssued { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(10,MinimumLength = 1 )]
        [JsonProperty("taxpayerActivityCode")]
        public string TaxpayerActivityCode { get; set; }
        [Required]
        [StringLength(50,MinimumLength = 1)]
        [JsonProperty("internalID")]
        public string InternalId { get; set; }
        [StringLength(100)]
        [JsonProperty("purchaseOrderReference")]
        public string PurchaseOrderReference { get; set; }
        [StringLength(500)]
        [JsonProperty("purchaseOrderDescription")]
        public string PurchaseOrderDescription { get; set; }
        [StringLength(100)]
        [JsonProperty("salesOrderReference")]
        public string SalesOrderReference { get; set; }
        [StringLength(500)]
        [JsonProperty("salesOrderDescription")]
        public string SalesOrderDescription { get; set; }
        [MaxLength(50)]
        [JsonProperty("proformaInvoiceNumber")]
        public string ProformaInvoiceNumber { get; set; }
        [JsonProperty("payment")]
        public Payment Payment { get; set; }
        [JsonProperty("delivery")]
        public Delivery Delivery { get; set; }
        [JsonProperty("invoiceLines")]
        public IList<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
        [JsonProperty("totalSalesAmount")]
        public double TotalSalesAmount { get; set; }
        [JsonProperty("totalDiscountAmount")]
        public double TotalDiscountAmount { get; set; }
        [JsonProperty("netAmount")]
        public double NetAmount { get; set; }
        [JsonProperty("taxTotals")]
        public IList<TaxTotal> TaxTotals { get; set; }
        [JsonProperty("extraDiscountAmount")]
        public double ExtraDiscountAmount { get; set; }
        [JsonProperty("totalItemsDiscountAmount")]
        public double TotalItemsDiscountAmount { get; set; }
        [JsonProperty("totalAmount")]
        public double TotalAmount { get; set; }
        [JsonProperty("signatures")]
        public IList<Signature> Signatures { get; set; } = new List<Signature>();
        [JsonIgnore]
        public byte[] Version { get; set; }
    }
}
