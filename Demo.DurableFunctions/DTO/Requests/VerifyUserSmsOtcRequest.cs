namespace Demo.DurableFunctions.DTO.Requests
{
    public class VerifyUserSmsOtcRequest
    {
        public string Id { get; set; }
        public string Code { get; set; }
    }
}