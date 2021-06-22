using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Core.Domain.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class CheckVisaActivity
    {
        private readonly ICheckVisaService checkVisaService;

        public CheckVisaActivity(ICheckVisaService checkVisaService)
        {
            this.checkVisaService = checkVisaService;
        }
        
        [FunctionName(nameof(CheckVisaActivity))]
        public async Task<Result> CheckVisaAsync([ActivityTrigger] IDurableActivityContext context)
        {
            var request = context.GetInput<CheckVisaRequest>();

            var operation = await checkVisaService.CheckAsync(request);

            return operation;
        }
    }
}