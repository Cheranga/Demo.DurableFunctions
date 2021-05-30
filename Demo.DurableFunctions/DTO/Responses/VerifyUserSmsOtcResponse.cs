using System;

namespace Demo.DurableFunctions.DTO.Responses
{
    public class VerifyUserSmsOtcResponse
    {
        public string Mobile { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}