using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Functions.Orchestrators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace Demo.DurableFunctions.Patterns.Eternal
{
    public class EternalCheckDocumentsClient
    {
        private readonly IHttpRequestBodyReader requestBodyReader;
        private readonly IValidator<CheckDocumentsRequest> validator;

        public EternalCheckDocumentsClient(IHttpRequestBodyReader requestBodyReader, IValidator<CheckDocumentsRequest> validator)
        {
            this.requestBodyReader = requestBodyReader;
            this.validator = validator;
        }
        
        [FunctionName(nameof(EternalCheckDocumentsClient))]
        public async Task<IActionResult> CheckAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "eternal")]
            HttpRequestMessage request,
            [DurableClient]IDurableClient client)
        {
            var checkDocumentsRequest = await requestBodyReader.GetModelAsync<CheckDocumentsRequest>(request);
            var validationResult = await validator.ValidateAsync(checkDocumentsRequest);

            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult("Invalid check document request");
            }

            var instanceId = await client.StartNewAsync(nameof(CheckDocumentsOrchestrator), checkDocumentsRequest);

            var httpResponse = client.CreateCheckStatusResponse(request, instanceId);
            var statusModel = JsonConvert.DeserializeObject(await httpResponse.Content.ReadAsStringAsync());

            var responseModel = new
            {
                instanceId,
                statusModel
            };
              
            return new OkObjectResult(responseModel);
        }
    }
}