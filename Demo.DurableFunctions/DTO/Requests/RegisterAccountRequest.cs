using System;
using Newtonsoft.Json;

namespace Demo.DurableFunctions.DTO.Requests
{
    public class RegisterAccountRequest
    {
        public string PassportNo { get; set; }
        public string DriverLicenseNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string AccountName { get; set; }
        public BankAccountType BankAccountType { get; set; }
        public decimal Deposit { get; set; }
    }
}