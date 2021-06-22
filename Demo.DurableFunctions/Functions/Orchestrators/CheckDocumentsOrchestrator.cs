using System.Threading;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Functions.Activities;
using Demo.DurableFunctions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Functions.Orchestrators
{
    public class CheckDocumentsOrchestrator
    {
        private readonly ILogger<CheckDocumentsOrchestrator> logger;

        public CheckDocumentsOrchestrator(ILogger<CheckDocumentsOrchestrator> logger)
        {
            this.logger = logger;
        }
        [FunctionName(nameof(CheckDocumentsOrchestrator))]
        public async Task<Result> CheckDocumentsAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var request = context.GetInput<CheckDocumentsRequest>();
            var operation = await context.CallActivityAsync<Result<int>>(nameof(CheckDocumentsActivity), request);
            if (!operation.Status)
            {
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(2), CancellationToken.None);
                context.ContinueAsNew(request);
                return Result.Failure(operation.ErrorCode, operation.ValidationResult);
            }

            var remainingDocumentCount = operation.Data;
            if (remainingDocumentCount != 0)
            {
                var RemainingCheckDocumentRequest = new CheckDocumentsRequest
                {
                    DocumentCount = remainingDocumentCount
                };
                
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(5), CancellationToken.None);
                context.ContinueAsNew(RemainingCheckDocumentRequest);
                return Result.Success();
            }

            return Result.Success();
        }
    }
}