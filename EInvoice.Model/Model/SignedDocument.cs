using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EInvoice.Model
{
    public class SignedDocument
    {
        public TaxAuthorityDocument taxAuthorityDocument { get;set; }
        [JsonProperty("issuer")]
        public Issuer Issuer { get; set; }
        [JsonProperty("receiver")]
        public Receiver Receiver { get; set; }
        [JsonProperty("documentType")]
        public string DocumentType { get; set; }
        [JsonProperty("documentTypeVersion")]
        public string DocumentTypeVersion { get; set; }
        [JsonProperty("dateTimeIssued")]
        public DateTime DateTimeIssued { get; set; }
        [JsonProperty("taxpayerActivityCode")]
        public string TaxpayerActivityCode { get; set; }
       
        [JsonProperty("internalID")]
        public string InternalId { get; set; }
 
        [JsonProperty("purchaseOrderReference")]
        public string PurchaseOrderReference { get; set; }

        [JsonProperty("purchaseOrderDescription")]
        public string PurchaseOrderDescription { get; set; }
  
        [JsonProperty("salesOrderReference")]
        public string SalesOrderReference { get; set; }
    
        [JsonProperty("salesOrderDescription")]
        public string SalesOrderDescription { get; set; }
 
        [JsonProperty("proformaInvoiceNumber")]
        public string ProformaInvoiceNumber { get; set; }
        [JsonProperty("payment")]
        public Payment Payment { get; set; }
        [JsonProperty("delivery")]
        public Delivery Delivery { get; set; }
        [JsonProperty("invoiceLines")]
        public IList<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
        [JsonProperty("totalSalesAmount")]
        public decimal TotalSalesAmount { get; set; }
        [JsonProperty("totalDiscountAmount")]
        public decimal TotalDiscountAmount { get; set; }
        [JsonProperty("netAmount")]
        public decimal NetAmount { get; set; }
        [JsonProperty("taxTotals")]
        public IList<TaxTotal> TaxTotals { get; set; }
        [JsonProperty("extraDiscountAmount")]
        public decimal ExtraDiscountAmount { get; set; }
        [JsonProperty("totalItemsDiscountAmount")]
        public decimal TotalItemsDiscountAmount { get; set; }
        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }
        [JsonProperty("signatures")]
        public IList<Signature> Signatures { get; set; } = new List<Signature>();
    }
}
