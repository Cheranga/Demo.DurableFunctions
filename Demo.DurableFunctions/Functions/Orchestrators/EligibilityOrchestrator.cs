using System;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Constants;
using Demo.DurableFunctions.Core.Application.Exceptions;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Extensions;
using Demo.DurableFunctions.Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Functions.Orchestrators
{
    public class EligibilityOrchestrator
    {
        private const string CheckVisa = nameof(CheckVisaActivity);
        private const string CheckDriverLicense = nameof(CheckDriverLicenseActivity);
        private readonly ILogger<EligibilityOrchestrator> logger;
        private readonly IMapper mapper;

        public EligibilityOrchestrator(IMapper mapper, ILogger<EligibilityOrchestrator> logger)
        {
            this.mapper = mapper;
            this.logger = logger;
        }

        [FunctionName(nameof(EligibilityOrchestrator))]
        public async Task<Result> CheckEligibilityAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            try
            {
                var request = context.GetInput<RegisterAccountRequest>();

                var checkVisaRequest = mapper.Map<CheckVisaRequest>(request);
                var checkDriverLicenseRequest = mapper.Map<CheckDriverLicenseRequest>(request);

                // Fan-out
                var checkVisaTask = context.CallActivityWithRetryAsync<Result>(CheckVisa, Retry.For<VisaServiceException>(), checkVisaRequest);
                var checkDriverLicenseTask = context.CallActivityWithRetryAsync<Result>(CheckDriverLicense, Retry.For<DriverLicenseServiceException>(), checkDriverLicenseRequest);
                await Task.WhenAll(checkVisaTask, checkDriverLicenseTask);

                // Fan-in
                var checkVisaOperation = checkVisaTask.Result;
                var checkDriverLicenseOperation = checkDriverLicenseTask.Result;

                if (checkVisaOperation.Status && checkDriverLicenseOperation.Status)
                {
                    return Result.Success();
                }

                if (!checkVisaOperation.Status)
                {
                    return Result.Failure(ErrorCodes.InvalidVisa, ErrorMessages.InvalidVisa);
                }

                return Result.Failure("InvalidDriverLicense", "invalid driver license");
            }
            catch (FunctionFailedException exception)
            {
                var errorCode = exception.InnerException.GetType() == typeof(DriverLicenseServiceException) ? ErrorCodes.DriverLicenseError : ErrorCodes.InvalidVisa;
                return Result.Failure(errorCode, exception.InnerException.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error occurred when checking for eligibility");
                return Result.Failure("EligibilityError", "cannot verify eligibility");
            }
        }
    }
}