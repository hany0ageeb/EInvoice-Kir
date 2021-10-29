using System.Collections.Generic;
using EInvoice.Model;
namespace EInvoice.Validation
{
    public class ValidationResult
    {
        public ValidationState ValidationState { get; set; }
        public IList<Error> Errors { get; set; } = new List<Error>();
        public override string ToString()
        {
            switch (ValidationState)
            {
                case ValidationState.Invalid:
                    return "Invalid";
                default:
                    return "Valid";
            }
        }
    }
}
