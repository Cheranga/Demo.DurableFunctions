using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Patterns.FunctionChaining;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Api
{
    public class FunctionChainingClientFunction
    {
        private readonly IHttpRequestBodyReader requestBodyReader;
        private readonly IValidator<RegisterBankCustomerRequest> validator;

        public FunctionChainingClientFunction(IHttpRequestBodyReader requestBodyReader, IValidator<RegisterBankCustomerRequest> validator)
        {
            this.requestBodyReader = requestBodyReader;
            this.validator = validator;
        }

        [FunctionName(nameof(FunctionChainingClientFunction))]
        public async Task<IActionResult> FunctionChainingAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chaining")]
            HttpRequestMessage request, [DurableClient] IDurableOrchestrationClient client)
        {
            var registerBankCustomerRequest = await requestBodyReader.GetModelAsync<RegisterBankCustomerRequest>(request);
            var validationResult = await validator.ValidateAsync(registerBankCustomerRequest);
            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(validationResult);
            }

            var instanceId = await client.StartNewAsync(nameof(CustomerAccountHandlerFunction), registerBankCustomerRequest);
            return new OkObjectResult(instanceId);
        }
    }
}