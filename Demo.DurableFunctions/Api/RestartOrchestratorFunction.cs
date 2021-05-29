using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Functions.Orchestrators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Api
{
    public class RestartOrchestratorFunction
    {
        private readonly IHttpRequestBodyReader requestBodyReader;

        public RestartOrchestratorFunction(IHttpRequestBodyReader requestBodyReader)
        {
            this.requestBodyReader = requestBodyReader;
        }
        
        [FunctionName(nameof(RestartOrchestratorFunction))]
        public async Task<IActionResult> RestartAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "restart/{instanceId}")]
            HttpRequestMessage request, string instanceId, [DurableClient]IDurableOrchestrationClient client)
        {
            var registerBankCustomerRequest = await requestBodyReader.GetModelAsync<RegisterAccountRequest>(request);

            var newInstanceId = await client.StartNewAsync(nameof(RegisterAccountOrchestrator), instanceId, registerBankCustomerRequest);
            return new OkObjectResult(newInstanceId);
        }
    }
}