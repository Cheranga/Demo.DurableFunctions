using Demo.DurableFunctions.Core.Domain.Requests;

namespace Demo.DurableFunctions.Core.Application.DataAccess
{
    public class CreateBankAccountCommand : ICommand
    {
        public string CustomerId { get; set; }
        public string BankAccountId { get; set; }
        public BankAccountType BankAccountType { get; set; }    
        public decimal Amount { get; set; }
    }
}