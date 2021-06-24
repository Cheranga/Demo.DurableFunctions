using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Bindings;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Core.Domain.Responses;
using Demo.DurableFunctions.Functions.Orchestrators;
using Demo.DurableFunctions.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Patterns.FanOutAndFanInWithChaining
{
    public class AsyncFunctionChainingClient
    {
        private const string Orchestrator = nameof(RegisterAccountOrchestrator);
        private readonly IHttpRequestBodyReader requestBodyReader;
        private readonly IValidator<RegisterAccountRequest> validator;

        public AsyncFunctionChainingClient(IHttpRequestBodyReader requestBodyReader, IValidator<RegisterAccountRequest> validator)
        {
            this.requestBodyReader = requestBodyReader;
            this.validator = validator;
        }

        [FunctionName(nameof(AsyncFunctionChainingClient))]
        public async Task<IActionResult> FunctionChainingAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chaining/async")]
            HttpRequestMessage request,
            [AzureAdToken("create.user","")]AzureAdToken token,
            [DurableClient] IDurableOrchestrationClient client)
        {
            if (token == null)
            {
                return new UnauthorizedResult();
            }
            
            var registerBankCustomerRequest = await requestBodyReader.GetModelAsync<RegisterAccountRequest>(request);
            var validationResult = await validator.ValidateAsync(registerBankCustomerRequest);
            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(validationResult);
            }

            var instanceId = await client.StartNewAsync(Orchestrator, registerBankCustomerRequest);
            var response = new RegisterAccountResponse
            {
                InstanceId = instanceId
            };

            return new ObjectResult(response)
            {
                StatusCode = (int) HttpStatusCode.Accepted
            };
        }
    }
}