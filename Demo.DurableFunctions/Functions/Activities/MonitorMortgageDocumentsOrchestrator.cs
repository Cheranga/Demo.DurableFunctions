using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class MonitorMortgageDocumentsOrchestrator
    {
        [FunctionName(nameof(MonitorMortgageDocumentsOrchestrator))]
        public async Task<Result> MonitorAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var timeout = context.CurrentUtcDateTime.AddSeconds(10);

            while (context.CurrentUtcDateTime < timeout)
            {
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(2), CancellationToken.None);
                
                var operation = await context.CallActivityAsync<Result>(nameof(CheckVisaActivity), new CheckVisaRequest
                {
                    PassportNo = "ABC123456"
                });

                if (operation.Status)
                {
                    return operation;
                }

                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(2), CancellationToken.None);
            }

            return Result.Failure("LateDocumentSubmission");
        }
    }
}