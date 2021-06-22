namespace Demo.DurableFunctions.Core.Application.Requests
{
    public class UpdateCustomerRequest
    {
        public string CustomerId { get; set; }
        public bool MobileVerified { get; set; }
    }
}