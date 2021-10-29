namespace EInvoice.Validation
{
    public interface IValidator<T>
    {
        ValidationResult IsValid(T document);
    }
}
