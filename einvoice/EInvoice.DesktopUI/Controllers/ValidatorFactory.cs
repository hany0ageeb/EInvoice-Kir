using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.Model;
using EInvoice.Validation;
namespace EInvoice.DesktopUI.Controllers
{
    public static class ValidatorFactory
    {
        private static IList<IValidator<Document>> validators = null;
        public static IList<IValidator<Document>> CreateValidators(IList<ActivityType> activityTypes,IList<CountryCode> countryCodes,IList<TaxType> taxTypes,double maxDocumentAmount,int invoiceSubmissionInHours)
        {
            if (validators == null)
            {
                validators = new List<IValidator<Document>>();
                validators.Add(new SimpleFieldValidator(invoiceSubmissionInHours));
                validators.Add(new CoreFieldsValidator());
                validators.Add(new CodeValidator(activityTypes, countryCodes, taxTypes));
                validators.Add(new NationalIDValidator(maxDocumentAmount));
            }
            return validators;
        }
    }
}
