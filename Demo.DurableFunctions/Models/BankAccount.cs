using Demo.DurableFunctions.DTO.Requests;

namespace Demo.DurableFunctions.Models
{
    public class BankAccount
    {
        public string Id { get; set; }
        public BankAccountType BankAccountType { get; set; }
    }
}