using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class CreateBankAccountFunction
    {
        [FunctionName(nameof(CreateBankAccountFunction))]
        public async Task CreateBankAccountAsync([ActivityTrigger] IDurableActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}