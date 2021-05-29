using System;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.DTO.Responses;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Extensions;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class RegisterAccountOrchestrator
    {
        private const string CreateCustomer = nameof(CreateCustomerActivity);
        private const string CreateBankAccount = nameof(CreateBankAccountActivity);
        private readonly IMapper mapper;
        private readonly ILogger<RegisterAccountOrchestrator> logger;

        public RegisterAccountOrchestrator(IMapper mapper, ILogger<RegisterAccountOrchestrator> logger)
        {
            this.mapper = mapper;
            this.logger = logger;
        }

        [FunctionName(nameof(RegisterAccountOrchestrator))]
        public async Task<Result<RegisterAccountResponse>> HandleCustomerAccountAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            try
            {
                var request = context.GetInput<RegisterAccountRequest>();
                
                var customerData = await RegisterAndGetCustomerData(context, request);
                var bankAccountData = await RegisterAndGetBankAccountData(context, request, customerData.Id);

                var response = new RegisterAccountResponse
                {
                    CustomerId = customerData.Id,
                    AccountId = bankAccountData.Id
                };

                return Result<RegisterAccountResponse>.Success(response);
            }
            catch (FunctionFailedException exception)
            {
                // You can choose to select to throw an exception or return a result. If you return a result the orchestrator sets the status as `Completed`, else it'll set it's status as `Failed`.
                // In here I have chosen to return a status object.
                // throw new CustomerAccountOrchestratorException(context.InstanceId);

                var errorCode = exception.InnerException.GetType() == typeof(CreateCustomerException) ? "CreateCustomerError" : "CreateBankAccountError";
                return Result<RegisterAccountResponse>.Failure(errorCode);
            }
        }

        private async Task<BankAccount> RegisterAndGetBankAccountData(IDurableOrchestrationContext context, RegisterAccountRequest request, string customerId)
        {
            Log(context, LogLevel.Information, "Registering bank account @{RegisterAccountRequest}", request);

            var createBankAccountRequest = mapper.Map<CreateBankAccountRequest>(request);
            createBankAccountRequest.CustomerId = customerId;

            var bankAccountData = await context.CallActivityWithRetryAsync<BankAccount>(CreateBankAccount, Retry.For<CreateBankAccountException>(), createBankAccountRequest);
            Log(context, LogLevel.Information, "Successfully registered bank account @{RegisterAccountRequest}", request);
            return bankAccountData;
        }

        private async Task<Customer> RegisterAndGetCustomerData(IDurableOrchestrationContext context, RegisterAccountRequest request)
        {
            Log(context, LogLevel.Information,"Registering customer @{RegisterCustomerRequest}", request);
            var createCustomerRequest = mapper.Map<CreateCustomerRequest>(request);
            var customerData = await context.CallActivityWithRetryAsync<Customer>(CreateCustomer, Retry.For<CreateCustomerException>(), createCustomerRequest);
            
            Log(context, LogLevel.Information,"Successfully registered customer @{RegisterCustomerRequest}", request);
            return customerData;
        }

        private void Log(IDurableOrchestrationContext context, LogLevel level, string message, params object[] arguments)
        {
            if (!context.IsReplaying)
            {
                logger.Log(level, message, arguments);
            }
        }
    }
}