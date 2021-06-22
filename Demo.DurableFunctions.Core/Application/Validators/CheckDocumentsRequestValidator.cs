using Demo.DurableFunctions.Core.Application.Requests;
using Demo.DurableFunctions.Core.Application.Validators;
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