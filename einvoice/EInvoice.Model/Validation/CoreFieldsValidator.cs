using System.Collections.Generic;
using EInvoice.Model;
namespace EInvoice.Validation
{
    public class CoreFieldsValidator : IValidator<Document>
    {
        private readonly ValidationResult _validationResult;

        public CoreFieldsValidator()
        {
            _validationResult = new ValidationResult() { ValidationState = ValidationState.Valid};
        }
        private void ValidateIssuer(Issuer issuer)
        {
            if (string.IsNullOrEmpty(issuer.Type))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Undefined Issuer.", Target = "issuer/type" });
            }
            else
            {
                if (!Issuer.IsValidIssuerType(issuer.Type))
                {
                    _validationResult.ValidationState = ValidationState.Invalid;
                    _validationResult.Errors.Add(new Error() { Code = "", Message = $"Issuer Type {issuer.Type} is invalid.", Target = "issuer/type" });
                }
            }
            if (string.IsNullOrEmpty(issuer.Id))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer Id.", Target = "issuer/id" });
            }
            if (string.IsNullOrEmpty(issuer.Name))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer Name.", Target = "issuer/name" });
            }
            if (issuer.Address == null)
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer Address.", Target = "issuer/address" });
            }
            else
            {
                ValidateIssuerAddress(issuer);
            }
        }
        private void ValidateIssuerAddress(Issuer issuer)
        {
            if (issuer.Type == "B" && string.IsNullOrEmpty(issuer.Address.BranchId))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer branch Id", Target = "issuer/address/branchId" });
            }
            if (string.IsNullOrEmpty(issuer.Address.Country))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer Country.", Target = "issuer/address/country" });
            }
            if (string.IsNullOrEmpty(issuer.Address.Governate))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer Country.", Target = "issuer/address/governate" });
            }
            if (string.IsNullOrEmpty(issuer.Address.RegionCity))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer Region City.", Target = "issuer/address/regionCity" });
            }
            if (string.IsNullOrEmpty(issuer.Address.Street))
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "", Message = $"Invalid Issuer street.", Target = "issuer/address/street" });
            }
        }
        public ValidationResult IsValid(Document document)
        {
            
            if(document.Issuer is null)
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code="",Message=$"Invalid Issuer.",Target="issuer"});
            }
            else
            {
                ValidateIssuer(document.Issuer);
            }
            if(document.Receiver is null)
            {
                _validationResult.ValidationState = ValidationState.Invalid;
                _validationResult.Errors.Add(new Error() { Code = "",Message=$"Invalid Receiver.",Target="receiver"});
            }
            else
            {
                ValidateReceiver(document.Receiver);
            }
            return new ValidationResult() { ValidationState = _validationResult.ValidationState, Errors = new List<Error>(_validationResult.Errors) };
        }
        private void ValidateReceiver(Receiver receiver)
        {

        }
    }
}
