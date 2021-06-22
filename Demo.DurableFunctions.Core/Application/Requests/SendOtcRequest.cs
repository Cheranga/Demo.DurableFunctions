namespace Demo.DurableFunctions.Core.Application.Requests
{
    public class SendOtcRequest
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Mobile { get; set; }
    }
}