using System;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Functions.Orchestrators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Patterns.HumanInteraction
{
    public class SendSmsOtcClient
    {
        private readonly IHttpRequestBodyReader requestBodyReader;

        public SendSmsOtcClient(IHttpRequestBodyReader requestBodyReader)
        {
            this.requestBodyReader = requestBodyReader;
        }
        
        [FunctionName(nameof(SendSmsOtcClient))]
        public async Task<IActionResult> SendSmsOtcAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "otc/send")]
            HttpRequestMessage request, [DurableClient]IDurableClient client)
        {
            var sendOtcCodeRequest = await requestBodyReader.GetModelAsync<SendOtcRequest>(request);

            var instanceId = Guid.NewGuid().ToString("N").ToUpper();
            sendOtcCodeRequest.Id = instanceId;
            
            await client.StartNewAsync(nameof(SmsOtcOrchestrator),instanceId, sendOtcCodeRequest);

            return new OkObjectResult(instanceId);
        }
        
    }
}