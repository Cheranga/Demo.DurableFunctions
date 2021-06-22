using System;

namespace Demo.DurableFunctions.Core.Domain.Responses
{
    public class VerifyUserSmsOtcResponse
    {
        public string Mobile { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}