using System;

namespace Demo.DurableFunctions.Core.Application.Exceptions
{
    public class VisaServiceException : Exception
    {
        private readonly string passportNo;

        public VisaServiceException(string passportNo)
        {
            this.passportNo = passportNo;
        }

        public override string Message => $"Cannot check passport for {passportNo}";
    }
}