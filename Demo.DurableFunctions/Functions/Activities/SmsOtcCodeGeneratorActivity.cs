using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class SmsOtcCodeGeneratorActivity
    {
        private readonly Random random = new Random();
        
        [FunctionName(nameof(SmsOtcCodeGeneratorActivity))]
        public Task<Result<string>> GenerateOtcAsync([ActivityTrigger]IDurableActivityContext context)
        {
            var otcCode = random.Next(100000, (999999 + 1));
            return Task.FromResult(Result<string>.Success(otcCode.ToString()));
        }
    }
}