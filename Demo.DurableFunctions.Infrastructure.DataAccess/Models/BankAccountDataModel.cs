using Demo.DurableFunctions.Core.Domain.Requests;
using Microsoft.Azure.Cosmos.Table;

namespace Demo.DurableFunctions.Infrastructure.DataAccess.Models
{
    public class BankAccountDataModel : TableEntity
    {
        public string CustomerId { get; set; }
        public string BankAccountId { get; set; }
        public BankAccountType BankAccountType { get; set; }    
        public decimal Amount { get; set; }
    }
}