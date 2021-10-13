using System;
namespace EInvoice.Model
{
    [Serializable]
    public class InvalidIssuerReceiverTypeException : Exception
    {
        public InvalidIssuerReceiverTypeException() : base() { }
        public InvalidIssuerReceiverTypeException(string message) : base(message) { }
        public InvalidIssuerReceiverTypeException(string message,Exception exception) : base(message, exception) { }
    }
}
