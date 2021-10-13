using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;

namespace EInvoice.Validation
{
    public static class ValidatorFactory
    {
        private static IList<IValidator<Document>> validators = null;
        public static IList<IValidator<Document>> CreateValidators(IList<ActivityType> activityTypes,IList<CountryCode> countryCodes,IList<TaxType> taxTypes,double maxInvoiceAmount = 5000,int invoiceSubmissionInHours = 170)
        {
            if (validators == null)
            {
                validators = new List<IValidator<Document>>();
                validators.Add(new SimpleFieldValidator(invoiceSubmissionInHours));
                validators.Add(new CoreFieldsValidator());
                validators.Add(new CodeValidator(activityTypes, countryCodes, taxTypes));
                validators.Add(new NationalIDValidator(maxInvoiceAmount));
            }
            return validators;
        }
    }
}
