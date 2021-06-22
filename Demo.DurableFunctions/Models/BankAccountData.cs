using Demo.DurableFunctions.Core.Domain.Requests;

namespace Demo.DurableFunctions.Models
{
    public class BankAccountData
    {
        public string Id { get; set; }
        public BankAccountType BankAccountType { get; set; }
    }
}