using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class AsyncFunctionChainingClientFunction
    {
        private const string Orchestrator = nameof(RegisterAccountOrchestrator);
        private readonly IHttpRequestBodyReader requestBodyReader;
        private readonly IValidator<RegisterAccountRequest> validator;

        public AsyncFunctionChainingClientFunction(IHttpRequestBodyReader requestBodyReader, IValidator<RegisterAccountRequest> validator)
        {
            this.requestBodyReader = requestBodyReader;
            this.validator = validator;
        }

        [FunctionName(nameof(AsyncFunctionChainingClientFunction))]
        public async Task<IActionResult> FunctionChainingAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chaining/async")]
            HttpRequestMessage request, [DurableClient] IDurableOrchestrationClient client)
        {
            var registerBankCustomerRequest = await requestBodyReader.GetModelAsync<RegisterAccountRequest>(request);
            var validationResult = await validator.ValidateAsync(registerBankCustomerRequest);
            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(validationResult);
            }
            
            var instanceId = await client.StartNewAsync(Orchestrator, registerBankCustomerRequest);
            return new OkObjectResult(instanceId);
        }
    }
}