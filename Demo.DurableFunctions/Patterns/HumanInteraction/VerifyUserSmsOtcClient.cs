using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.DTO.Responses;
using Demo.DurableFunctions.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Patterns.HumanInteraction
{
    public class VerifyUserSmsOtcClient
    {
        private readonly IHttpRequestBodyReader requestBodyReader;

        public VerifyUserSmsOtcClient(IHttpRequestBodyReader requestBodyReader)
        {
            this.requestBodyReader = requestBodyReader;
        }

        [FunctionName(nameof(VerifyUserSmsOtcClient))]
        public async Task<IActionResult> VerifyUserSmsOtcAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "otc/verify")]
            HttpRequestMessage request,
            [DurableClient] IDurableClient client)
        {
            var verifyUserSmsOtcRequest = await requestBodyReader.GetModelAsync<VerifyUserSmsOtcRequest>(request);
            var instanceId = verifyUserSmsOtcRequest.Id;

            var orchestratorStatus = await client.GetStatusAsync(instanceId, false, false, false);
            if (orchestratorStatus.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
            {
                var operation = orchestratorStatus.Output.ToObject<Result<VerifyUserSmsOtcResponse>>();
                if (operation.Status)
                {
                    return new OkObjectResult(operation.Data);
                }

                return new ObjectResult(operation.ToErrorResponse())
                {
                    StatusCode = (int) (HttpStatusCode.InternalServerError)
                };
            }
            await client.RaiseEventAsync(instanceId, "SmsChallengeResponse", verifyUserSmsOtcRequest);

            return new AcceptedResult();

        }
    }
}