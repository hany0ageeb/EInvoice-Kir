namespace EInvoice.Model
{
    public class InvoiceLineItemCode
    {
        public int codeTypeId { get; set; }
        public string codeTypeNamePrimaryLang { get; set; }
        public string codeTypeNameSecondaryLang { get; set; }
        public string itemCode { get; set; }
        public string codeNamePrimaryLang { get; set; }
        public string codeNameSecondaryLang { get; set; }
    }
}
