namespace EInvoice.Model
{
    public class ValidationStepResult
    {
        public string name { get; set; }
        public string status { get; set; }
        public Error error { get; set; }
    }
}
