using System;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.DTO.Responses;
using Demo.DurableFunctions.Functions.Orchestrators;
using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Services
{
    public class RegisterBankAccountService : IRegisterBankAccountService
    {
        private const int TimeoutInSeconds = 120;
        
        private readonly IHttpRequestBodyReader requestBodyReader;
        private readonly IValidator<RegisterAccountRequest> validator;

        public RegisterBankAccountService(IHttpRequestBodyReader requestBodyReader, IValidator<RegisterAccountRequest> validator)
        {
            this.requestBodyReader = requestBodyReader;
            this.validator = validator;
        }

        public async Task<Result<RegisterAccountResponse>> RegisterAsync(HttpRequestMessage request, IDurableClient client)
        {
            var registerAccountRequest = await requestBodyReader.GetModelAsync<RegisterAccountRequest>(request);
            var validationResult = await validator.ValidateAsync(registerAccountRequest);
            if (!validationResult.IsValid)
            {
                return Result<RegisterAccountResponse>.Failure("InvalidRequest", validationResult);
            }

            var timeout = TimeSpan.FromSeconds(TimeoutInSeconds);
            var instanceId = await client.StartNewAsync(nameof(RegisterAccountOrchestrator), registerAccountRequest);

            await client.WaitForCompletionOrCreateCheckStatusResponseAsync(request, instanceId, timeout);

            var orchestratorResult = await client.GetStatusAsync(instanceId, false, false, false);
            if (orchestratorResult.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
            {
                var operation = orchestratorResult.Output.ToObject<Result<RegisterAccountResponse>>();
                if (operation.Data != null)
                {
                    operation.Data.InstanceId = instanceId;
                }
                
                return operation;
            }

            await client.TerminateAsync(instanceId, "Timeout occured");
            return Result<RegisterAccountResponse>.Failure("Timeout");
        }
    }
}