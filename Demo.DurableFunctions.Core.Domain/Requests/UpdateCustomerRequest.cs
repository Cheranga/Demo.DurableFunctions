namespace Demo.DurableFunctions.Core.Domain.Requests
{
    public class UpdateCustomerRequest
    {
        public string CustomerId { get; set; }
        public bool MobileVerified { get; set; }
    }
}