using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using FluentValidation;

namespace Demo.DurableFunctions.Validators
{
    public class RegisterBankCustomerRequestValidator : ModelValidatorBase<RegisterAccountRequest>
    {
        public RegisterBankCustomerRequestValidator()
        {
            RuleFor(x => x.CustomerEmail).NotNull().NotEmpty();
            RuleFor(x => x.CustomerName).NotNull().NotEmpty();

            RuleFor(x => x.BankAccountType).IsInEnum();
            RuleFor(x => x.Deposit).GreaterThan(0);
            RuleFor(x => x.AccountName).NotNull().NotEmpty();
        }
    }
}