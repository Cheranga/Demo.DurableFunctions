namespace Demo.DurableFunctions.DTO.Requests
{
    public class SendOtcRequest
    {
        public string CustomerId { get; set; }
        public string Mobile { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string CustomerId { get; set; }
        public bool MobileVerified { get; set; }
    }
}