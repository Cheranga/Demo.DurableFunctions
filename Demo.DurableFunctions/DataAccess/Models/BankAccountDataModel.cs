using Demo.DurableFunctions.Core.Application.Requests;
using Microsoft.WindowsAzure.Storage.Table;

namespace Demo.DurableFunctions.DataAccess.Models
{
    public class BankAccountDataModel : TableEntity
    {
        public string CustomerId { get; set; }
        public string BankAccountId { get; set; }
        public BankAccountType BankAccountType { get; set; }    
        public decimal Amount { get; set; }
    }
}