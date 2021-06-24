using System;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Bindings;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Core.Domain.Responses;
using Demo.DurableFunctions.Functions.Orchestrators;
using Demo.DurableFunctions.Services;
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
            HttpRequestMessage request, 
            [AzureAdToken("sms.send","")]AzureAdToken token,
            [DurableClient]IDurableClient client)
        {
            if (token == null)
            {
                return new UnauthorizedResult();
            }
            
            var sendOtcCodeRequest = await requestBodyReader.GetModelAsync<SendOtcRequest>(request);

            var instanceId = Guid.NewGuid().ToString("N").ToUpper();
            sendOtcCodeRequest.Id = instanceId;
            
            await client.StartNewAsync(nameof(SmsOtcOrchestrator),instanceId, sendOtcCodeRequest);

            var response = new SendOtcResponse
            {
                InstanceId = instanceId
            };
            
            return new OkObjectResult(response);
        }
        
    }
}