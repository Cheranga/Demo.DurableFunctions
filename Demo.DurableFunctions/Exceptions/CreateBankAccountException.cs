using System;
using Demo.DurableFunctions.DTO.Requests;

namespace Demo.DurableFunctions.Exceptions
{
    public class CreateBankAccountException : Exception
    {
        private readonly BankAccountType accountType;

        public CreateBankAccountException(BankAccountType accountType)
        {
            this.accountType = accountType;
        }

        public override string Message => $"Cannot create bank account for {accountType}";
    }
}