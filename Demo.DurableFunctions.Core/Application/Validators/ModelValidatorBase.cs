using FluentValidation;
using FluentValidation.Results;

namespace Demo.DurableFunctions.Core.Application.Validators
{
    public class ModelValidatorBase<TModel> : AbstractValidator<TModel> where TModel : class
    {
        protected override bool PreValidate(ValidationContext<TModel> context, ValidationResult result)
        {
            var instance = context.InstanceToValidate;
            if (instance != null) return true;

            result.Errors.Add(new ValidationFailure("Instance", "Instance cannot be null"));
            return false;
        }
    }
}