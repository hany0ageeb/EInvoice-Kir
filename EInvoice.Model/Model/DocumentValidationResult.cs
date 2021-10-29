using System.Collections.Generic;

namespace EInvoice.Model
{
    public class DocumentValidationResult
    {
        public string status { get; set; }
        public IList<ValidationStepResult> validationSteps { get; set; }
    }
}
