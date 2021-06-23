using System;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Constants;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Core.Domain.Models;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Core.Domain.Responses;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Extensions;
using Demo.DurableFunctions.Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Functions.Orchestrators
{
    public class RegisterAccountOrchestrator
    {
        private const string CreateCustomer = nameof(CreateCustomerActivity);
        private const string CreateBankAccount = nameof(CreateBankAccountActivity);
        private const string CheckEligibility = nameof(EligibilityOrchestrator);

        private readonly ILogger<RegisterAccountOrchestrator> logger;
        private readonly IMapper mapper;

        public RegisterAccountOrchestrator(IMapper mapper, ILogger<RegisterAccountOrchestrator> logger)
        {
            this.mapper = mapper;
            this.logger = logger;
        }

        [FunctionName(nameof(RegisterAccountOrchestrator))]
        public async Task<Result<RegisterAccountResponse>> HandleCustomerAccountAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var request = context.GetInput<RegisterAccountRequest>();

            var eligibilityOperation = await context.CallSubOrchestratorAsync<Result>(CheckEligibility, request);
            if (!eligibilityOperation.Status)
            {
                return Result<RegisterAccountResponse>.Failure(eligibilityOperation.ErrorCode, eligibilityOperation.ValidationResult);
            }

            try
            {
                //
                // Function chaining
                //
                var customerData = await RegisterCustomerAsync(context, request);
                var bankAccountData = await RegisterBankAccountAsync(context, request, customerData.Id);

                var response = new RegisterAccountResponse
                {
                    InstanceId = context.InstanceId,
                    CustomerId = customerData.Id,
                    AccountId = bankAccountData.Id
                };

                return Result<RegisterAccountResponse>.Success(response);
            }
            catch (FunctionFailedException exception)
            {
                Log(context, LogLevel.Error, exception.Message);
                
                var errorCode = exception.InnerException.GetType() == typeof(CreateCustomerException) ? ErrorCodes.CreateCustomerError : ErrorCodes.CreateBankAccountError;
                return Result<RegisterAccountResponse>.Failure(errorCode, exception.Message);
            }
            catch (Exception exception)
            {
                Log(context, LogLevel.Error, exception.Message);

                return Result<RegisterAccountResponse>.Failure(ErrorCodes.RegisterAccountError, ErrorMessages.RegisterAccountError);
            }
        }

        private async Task<BankAccount> RegisterBankAccountAsync(IDurableOrchestrationContext context, RegisterAccountRequest request, string customerId)
        {
            Log(context, LogLevel.Information, "Registering bank account @{RegisterAccountRequest}", request);

            var createBankAccountRequest = mapper.Map<CreateBankAccountRequest>(request);
            createBankAccountRequest.CustomerId = customerId;

            var bankAccountData = await context.CallActivityWithRetryAsync<BankAccount>(CreateBankAccount, Retry.For<CreateBankAccountException>(), createBankAccountRequest);
            Log(context, LogLevel.Information, "Successfully registered bank account @{RegisterAccountRequest}", request);
            return bankAccountData;
        }

        private async Task<Customer> RegisterCustomerAsync(IDurableOrchestrationContext context, RegisterAccountRequest request)
        {
            Log(context, LogLevel.Information, "Registering customer @{RegisterCustomerRequest}", request);
            
            var createCustomerRequest = mapper.Map<CreateCustomerRequest>(request);
            var customerData = await context.CallActivityWithRetryAsync<Customer>(CreateCustomer, Retry.For<CreateCustomerException>(), createCustomerRequest);

            Log(context, LogLevel.Information, "Successfully registered customer @{RegisterCustomerRequest}", request);
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