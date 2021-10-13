using EInvoice.Model;
namespace EInvoice.Validation
{
    public class NationalIDValidator : IValidator<Document>
    {
        private readonly double _maximumAmount;
        public NationalIDValidator(double maxAmount)
        {
            _maximumAmount = maxAmount;
        }
        public ValidationResult IsValid(Document document)
        {
            var result = new ValidationResult() { ValidationState = ValidationState.Valid };
            if(document.TotalAmount > _maximumAmount && string.IsNullOrEmpty(document.Receiver.Id) && document.Receiver.Type==ReceiverType.P)
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = $"Invalid Receiver National Id", Target = "receiver/id" });
            }
            return result;
        }
    }
}
