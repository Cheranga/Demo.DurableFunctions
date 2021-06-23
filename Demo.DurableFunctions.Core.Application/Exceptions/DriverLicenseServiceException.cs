using System;

namespace Demo.DurableFunctions.Core.Application.Exceptions
{
    public class DriverLicenseServiceException : Exception
    {
        private readonly string driverLicenseNo;

        public DriverLicenseServiceException(string driverLicenseNo)
        {
            this.driverLicenseNo = driverLicenseNo;
        }

        public override string Message => $"Cannot check visa for {driverLicenseNo}";
    }
}