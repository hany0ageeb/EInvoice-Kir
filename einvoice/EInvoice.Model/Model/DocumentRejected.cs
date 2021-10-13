namespace EInvoice.Model
{
    public class DocumentRejected
    {
        public string internalId { get; set; }
        public Error error { get; set; }
    }
}
