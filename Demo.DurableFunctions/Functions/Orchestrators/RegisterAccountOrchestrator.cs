using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.DTO.Responses;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Extensions;
using Demo.DurableFunctions.Functions.Activities;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Functions.Orchestrators
{
    public class RegisterAccountOrchestrator
    {
        private const string CreateCustomer = nameof(CreateCustomerActivity);
        private const string CreateBankAccount = nameof(CreateBankAccountActivity);
        private const string CheckVisa = nameof(CheckVisaActivity);
        private const string CheckDriverLicense = nameof(CheckDriverLicenseActivity);
        private const string CheckEligibility = nameof(EligibilityOrchestrator);
        
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

                // Uncomment this for sub orchestration
                // var eligibilityOperation = await context.CallSubOrchestratorAsync<Result>(CheckEligibility, request);
                // if (!eligibilityOperation.Status)
                // {
                //     return Result<RegisterAccountResponse>.Failure(eligibilityOperation.ErrorCode);
                // }
                
                // Comment this for sub orchestration
                var checkVisaRequest = mapper.Map<CheckVisaRequest>(request);
                var checkDriverLicenseRequest = mapper.Map<CheckDriverLicenseRequest>(request);
                // Fan-out
                var checkVisaTask = context.CallActivityAsync<Result>(CheckVisa, checkVisaRequest);
                var checkDriverLicenseTask = context.CallActivityAsync<Result>(CheckDriverLicense, checkDriverLicenseRequest);
                await Task.WhenAll(checkVisaTask, checkDriverLicenseTask);
                
                // Fan-in
                var checkVisaOperation = checkVisaTask.Result;
                var checkDriverLicenseOperation = checkDriverLicenseTask.Result;
                if (!checkVisaOperation.Status)
                {
                    return Result<RegisterAccountResponse>.Failure("InvalidVisa");
                }
                
                if (!checkDriverLicenseOperation.Status)
                {
                    return Result<RegisterAccountResponse>.Failure("InvalidDriverLicense");
                }
                // Comment end
                
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

        private async Task<BankAccountData> RegisterAndGetBankAccountData(IDurableOrchestrationContext context, RegisterAccountRequest request, string customerId)
        {
            Log(context, LogLevel.Information, "Registering bank account @{RegisterAccountRequest}", request);

            var createBankAccountRequest = mapper.Map<CreateBankAccountRequest>(request);
            createBankAccountRequest.CustomerId = customerId;

            var bankAccountData = await context.CallActivityWithRetryAsync<BankAccountData>(CreateBankAccount, Retry.For<CreateBankAccountException>(), createBankAccountRequest);
            Log(context, LogLevel.Information, "Successfully registered bank account @{RegisterAccountRequest}", request);
            return bankAccountData;
        }

        private async Task<CustomerData> RegisterAndGetCustomerData(IDurableOrchestrationContext context, RegisterAccountRequest request)
        {
            Log(context, LogLevel.Information,"Registering customer @{RegisterCustomerRequest}", request);
            var createCustomerRequest = mapper.Map<CreateCustomerRequest>(request);
            var customerData = await context.CallActivityWithRetryAsync<CustomerData>(CreateCustomer, Retry.For<CreateCustomerException>(), createCustomerRequest);
            
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