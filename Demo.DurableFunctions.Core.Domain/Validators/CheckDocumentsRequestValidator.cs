using Demo.DurableFunctions.Core.Domain.Requests;
using FluentValidation;

namespace Demo.DurableFunctions.Core.Domain.Validators
{
    public class CheckDocumentsRequestValidator : ModelValidatorBase<CheckDocumentsRequest>
    {
        public CheckDocumentsRequestValidator()
        {
            RuleFor(x => x.DocumentCount).GreaterThanOrEqualTo(0);
        }
    }
}