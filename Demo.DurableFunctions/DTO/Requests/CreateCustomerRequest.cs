using System;

namespace Demo.DurableFunctions.DTO.Requests
{
    public class CreateCustomerRequest
    {
        public string PassportNo { get; set; }
        public string DriverLicenseNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
}