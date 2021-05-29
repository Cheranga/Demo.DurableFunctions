using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class CreateBankAccountActivity
    {
        [FunctionName(nameof(CreateBankAccountActivity))]
        public async Task<BankAccount> CreateBankAccountAsync([ActivityTrigger] IDurableActivityContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            var request = context.GetInput<CreateBankAccountRequest>();

            return new BankAccount
            {
                Id = Guid.NewGuid().ToString("N"),
                BankAccountType = request.BankAccountType
            };
        }
    }
}