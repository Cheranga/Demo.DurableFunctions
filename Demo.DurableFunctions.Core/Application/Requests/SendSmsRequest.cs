namespace Demo.DurableFunctions.Core.Application.Requests
{
    public class SendSmsRequest
    {
        public string Mobile { get; set; }
        public string Message { get; set; }
    }
}