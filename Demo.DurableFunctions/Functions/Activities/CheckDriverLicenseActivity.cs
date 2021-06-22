using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Core.Domain.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class CheckDriverLicenseActivity
    {
        private readonly ICheckDriverLicenseService checkDriverLicenseService;

        public CheckDriverLicenseActivity(ICheckDriverLicenseService checkDriverLicenseService)
        {
            this.checkDriverLicenseService = checkDriverLicenseService;
        }
        
        [FunctionName(nameof(CheckDriverLicenseActivity))]
        public async Task<Result> CheckVisaAsync([ActivityTrigger] IDurableActivityContext context)
        {
            var request = context.GetInput<CheckDriverLicenseRequest>();

            var operation = await checkDriverLicenseService.CheckAsync(request);

            return operation;
        }
    }
}