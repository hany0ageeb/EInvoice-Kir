using System.Collections.Generic;
using System.Linq;
using EInvoice.Model;
namespace EInvoice.Validation
{
    public class CodeValidator : IValidator<Document>
    {
        private readonly IList<ActivityType> activityTypes;
        private readonly IList<CountryCode> countryCodes;
        private readonly IList<TaxType> taxTypes;

        public CodeValidator(IList<ActivityType> activityTypes, IList<CountryCode> countryCodes, IList<TaxType> taxTypes)
        {
            this.activityTypes = activityTypes;
            this.countryCodes = countryCodes;
            this.taxTypes = taxTypes;
        }
        public ValidationResult IsValid(Document document)
        {
            var result = new ValidationResult() { ValidationState = ValidationState.Valid };
            ActivityType at = activityTypes.FirstOrDefault((activity) => { return activity.Code == document.TaxpayerActivityCode; });
            if(at == null)
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error(code: "", message: $"Invalid Tax Payer Activity {document.TaxpayerActivityCode}",details:null) { Target="" });
            }
            CountryCode cc = countryCodes.First((c) => { return c.Code == document.Issuer.Address.Country; });
            if(cc is null)
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer Country Code {document.Receiver.Address.Country}", Target = "" });
            }
            cc = countryCodes.First((c) => { return c.Code == document.Receiver.Address.Country; });
            if(cc is null)
            {
                result.ValidationState = ValidationState.Invalid;
                result.Errors.Add(new Error() { Code = "", Message = $"Invalid Receiver Country Code {document.Receiver.Address.Country}", Target = "" });
            }
            foreach(var taxTotal in document.TaxTotals)
            {
                var found = taxTypes.FirstOrDefault((tt) => 
                {
                    return tt.Code == taxTotal.TaxType;
                });
                if (found == null)
                {
                    result.ValidationState = ValidationState.Invalid;
                    result.Errors.Add(new Error("","Invalid Tax Type.",null) { Target="document/TaxTotals/TaxType"});
                }
            }
            foreach(InvoiceLine line in document.InvoiceLines)
            {
                foreach(TaxableItem taxableItem in line.TaxableItems)
                {
                    var query = (from tt in taxTypes where tt.Code == taxableItem.TaxType select tt).ToList();
                    if (query.Count == 0)
                    {
                        result.ValidationState = ValidationState.Invalid;
                        result.Errors.Add(new Error("", $"Invalid Tax Type. At Invoice Line Item {line.InternalCode}.", null) { Target = "document/InvoiceLines/TaxType" });
                    }
                    else
                    {
                        var subtypes = (from st in query.FirstOrDefault().SubType where st.Code == taxableItem.SubType select st).ToList();
                        if (subtypes.Count == 0)
                        {
                            result.ValidationState = ValidationState.Invalid;
                            result.Errors.Add(new Error("", $"Invalid Tax sub Type. At Invoice Line Item {line.InternalCode}.", null) { Target = "document/InvoiceLines/TaxType" });
                        }
                    }
                }
            }
            return result;
        }
    }
}
