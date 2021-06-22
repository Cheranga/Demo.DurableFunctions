namespace Demo.DurableFunctions.Core.Domain.Responses
{
    public class RegisterAccountResponse
    {
        public string InstanceId { get; set; }
        public string CustomerId { get; set; }
        public string AccountId { get; set; }
    }
}