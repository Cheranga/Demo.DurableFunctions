using System;

namespace Demo.DurableFunctions.Exceptions
{
    public class RegisterCustomerException : Exception
    {
        public RegisterCustomerException(string message): base(message)
        {
            
        }
    }
}