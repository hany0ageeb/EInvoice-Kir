namespace EInvoice.DesktopUI.ViewModel
{
    public class Settings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TokenIssuerName { get; set; }
        public double MaximumInvoiceTotalAmountWithoutNationalId { get; set; }
        public int InvoiceSubmissionInHours { get; set; }
        public string DLLPath { get; set; }
        public bool RemoteConnection { get; set; } = false;
        public bool EnableFileGeneration { get; set; } = false;
        public string SerializedFolderName { get; set; }
        public string SignedInvoicesFolderName { get; set; }
        public string RejectedInvoicesFolderName { get; set; }
    }
}
