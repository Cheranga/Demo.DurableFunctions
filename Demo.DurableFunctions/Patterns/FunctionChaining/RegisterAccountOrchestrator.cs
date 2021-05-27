using System.Threading.Tasks;
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
        private readonly ILogger<RegisterAccountOrchestrator> logger;

        public RegisterAccountOrchestrator(ILogger<RegisterAccountOrchestrator> logger)
        {
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
                // throw new CustomerAccountOrchestratorException(context.InstanceId);
                return Result<RegisterAccountResponse>.Failure(exception.Message);
            }
        }

        private async Task<BankAccount> RegisterAndGetBankAccountData(IDurableOrchestrationContext context, RegisterAccountRequest request, string customerId)
        {
            LogMessage(context, LogLevel.Information, "Registering bank account @{RegisterAccountRequest}", request);

            var createBankAccountRequest = new CreateBankAccountRequest
            {
                CustomerId = customerId,
                BankAccountType = request.BankAccountType,
                AccountName = request.AccountName,
                Amount = request.Deposit
            };
            var bankAccountData = await context.CallActivityWithRetryAsync<BankAccount>(CreateBankAccount, Retry.For<CreateBankAccountException>(), createBankAccountRequest);
            LogMessage(context, LogLevel.Information, "Successfully registered bank account @{RegisterAccountRequest}", request);
            return bankAccountData;
        }

        private async Task<Customer> RegisterAndGetCustomerData(IDurableOrchestrationContext context, RegisterAccountRequest request)
        {
            LogMessage(context, LogLevel.Information,"Registering customer @{RegisterCustomerRequest}", request);
            var createCustomerRequest = new RegisterCustomerRequest
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail
            };

            var customerData = await context.CallActivityWithRetryAsync<Customer>(CreateCustomer, Retry.For<CreateCustomerException>(), createCustomerRequest);
            
            LogMessage(context, LogLevel.Information,"Successfully registered customer @{RegisterCustomerRequest}", request);
            return customerData;
        }

        private void LogMessage(IDurableOrchestrationContext context, LogLevel level, string message, params object[] arguments)
        {
            if (!context.IsReplaying)
            {
                logger.Log(level, message, arguments);
            }
        }
    }
}