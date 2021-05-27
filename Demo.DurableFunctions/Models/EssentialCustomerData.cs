using Demo.DurableFunctions.DTO.Requests;

namespace Demo.DurableFunctions.Models
{
    public class Customer
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }

    public class BankAccount
    {
        public string Id { get; set; }
        public BankAccountType BankAccountType { get; set; }
    }
}