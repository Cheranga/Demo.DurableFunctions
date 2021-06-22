namespace Demo.DurableFunctions.Core.Domain.Requests
{
    public class VerifyUserSmsOtcRequest
    {
        public string Id { get; set; }
        public string Code { get; set; }
    }
}