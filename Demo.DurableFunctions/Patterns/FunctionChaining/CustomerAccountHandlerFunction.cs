using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class CustomerAccountHandlerFunction
    {
        private readonly RetryOptions retryOptions;

        public CustomerAccountHandlerFunction()
        {
            retryOptions = new RetryOptions(TimeSpan.FromSeconds(5), 3)
            {
                Handle = exception =>
                {
                    var status = exception?.InnerException?.GetType() == typeof(RegisterCustomerException);
                    return status;
                }
            };
        }
        
        [FunctionName(nameof(CustomerAccountHandlerFunction))]
        public async Task HandleCustomerAccountAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var request = context.GetInput<RegisterBankCustomerRequest>();

            var createCustomerRequest = new RegisterCustomerRequest
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail
            };

            var customerData = await context.CallActivityWithRetryAsync<EssentialCustomerData>(nameof(RegisterCustomerFunction), retryOptions, createCustomerRequest);
            

            // var validationResult = await validator.ValidateAsync(request);
            // if (!validationResult.IsValid)
            // {
            //     context.SetCustomStatus(new
            //     {
            //         context.InstanceId,
            //         Errors = validationResult
            //     });
            //
            //     context.SetOutput(validationResult);
            // }
        }
    }
}