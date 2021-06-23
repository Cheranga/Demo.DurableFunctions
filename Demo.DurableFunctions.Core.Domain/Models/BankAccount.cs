using Demo.DurableFunctions.Core.Domain.Requests;

namespace Demo.DurableFunctions.Core.Domain.Models
{
    public class BankAccount
    {
        public string Id { get; set; }
        public BankAccountType BankAccountType { get; set; }
    }
}