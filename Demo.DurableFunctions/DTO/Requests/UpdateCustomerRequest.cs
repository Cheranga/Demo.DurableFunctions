namespace Demo.DurableFunctions.DTO.Requests
{
    public class UpdateCustomerRequest
    {
        public string CustomerId { get; set; }
        public bool MobileVerified { get; set; }
    }
}