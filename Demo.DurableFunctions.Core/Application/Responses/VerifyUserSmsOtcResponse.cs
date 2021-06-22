using System;

namespace Demo.DurableFunctions.Core.Application.Responses
{
    public class VerifyUserSmsOtcResponse
    {
        public string Mobile { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}