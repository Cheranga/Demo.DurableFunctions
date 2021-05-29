using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Functions.Activities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Patterns.Monitor
{
    public class StartMortgageDocumentMonitorClientFunction
    {
        [FunctionName(nameof(StartMortgageDocumentMonitorClientFunction))]
        public async Task<IActionResult> StartMonitoringAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "mortgage/monitor")]
            HttpRequestMessage request,
            [DurableClient]IDurableClient client)
        {
            var instanceId = await client.StartNewAsync(nameof(MonitorMortgageDocumentsOrchestrator));
            return new AcceptedResult();
        }
    }
}