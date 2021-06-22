namespace Demo.DurableFunctions.Core.Application.Requests
{
    public class CreateCustomerRequest
    {
        public string PassportNo { get; set; }
        public string DriverLicenseNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
}