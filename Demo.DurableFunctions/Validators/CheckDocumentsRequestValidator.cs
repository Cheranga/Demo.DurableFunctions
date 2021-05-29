using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using FluentValidation;

namespace Demo.DurableFunctions.Validators
{
    public class CheckDocumentsRequestValidator : ModelValidatorBase<CheckDocumentsRequest>
    {
        public CheckDocumentsRequestValidator()
        {
            RuleFor(x => x.DocumentCount).GreaterThanOrEqualTo(0);
        }
    }
}