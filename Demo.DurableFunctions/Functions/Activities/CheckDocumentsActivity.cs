using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application.Requests;
using Demo.DurableFunctions.Core.Application.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class CheckDocumentsActivity
    {
        private readonly IVerifyDocumentsService verifyDocumentsService;

        public CheckDocumentsActivity(IVerifyDocumentsService verifyDocumentsService)
        {
            this.verifyDocumentsService = verifyDocumentsService;
        }
        
        [FunctionName(nameof(CheckDocumentsActivity))]
        public async Task<Result<int>> CheckDocumentsAsync([ActivityTrigger] IDurableActivityContext context)
        {
            var request = context.GetInput<CheckDocumentsRequest>();

            var operation = await verifyDocumentsService.VerifyAsync(request);
            return operation;
        }
    }
}