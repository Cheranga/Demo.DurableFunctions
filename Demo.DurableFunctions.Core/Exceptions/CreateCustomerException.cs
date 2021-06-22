using System;

namespace Demo.DurableFunctions.Core.Exceptions
{
    public class CreateCustomerException : Exception
    {
        private readonly string customerName;
        private readonly string customerEmail;

        public CreateCustomerException(string customerName, string customerEmail)
        {
            this.customerName = customerName;
            this.customerEmail = customerEmail;
        }

        public override string Message => $"Customer registration error for {customerName}, {customerEmail}";
    }
}