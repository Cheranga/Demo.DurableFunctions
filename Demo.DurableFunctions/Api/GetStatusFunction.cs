using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Api
{
    public class GetOrchestratorStatusFunction
    {
        [FunctionName(nameof(GetOrchestratorStatusFunction))]
        public async Task<IActionResult> GetStatusAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/{instanceId}")]
            HttpRequestMessage request, string instanceId, [DurableClient] IDurableOrchestrationClient client)
        {
            var orchestrationStatus = await client.GetStatusAsync(instanceId, true, true, true);
            return new OkObjectResult(orchestrationStatus);
        }
    }
}