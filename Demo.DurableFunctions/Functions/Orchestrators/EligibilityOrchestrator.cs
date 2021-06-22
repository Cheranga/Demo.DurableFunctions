using System;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application.Requests;
using Demo.DurableFunctions.Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Functions.Orchestrators
{
    public class EligibilityOrchestrator
    {
        private readonly IMapper mapper;
        private readonly ILogger<EligibilityOrchestrator> logger;
        private const string CheckVisa = nameof(CheckVisaActivity);
        private const string CheckDriverLicense = nameof(CheckDriverLicenseActivity);

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
                var checkVisaTask = context.CallActivityAsync<Result>(CheckVisa, checkVisaRequest);
                var checkDriverLicenseTask = context.CallActivityAsync<Result>(CheckDriverLicense, checkDriverLicenseRequest);
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
                    return Result.Failure("InvalidVisa", "invalid visa");
                }

                return Result.Failure("InvalidDriverLicense", "invalid driver license");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error occured when checking for eligibility");
                return Result.Failure("EligibilityError", "cannot verify eligibility");
            }
        }
    }
}