using System.Threading;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.DTO.Responses;
using Demo.DurableFunctions.Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Orchestrators
{
    public class SmsOtcOrchestrator
    {
        [FunctionName(nameof(SmsOtcOrchestrator))]
        public async Task<Result<VerifyUserSmsOtcResponse>> SendSmsOtcAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var request = context.GetInput<SendOtcRequest>();

            var otcCodeOperation = await context.CallActivityAsync<Result<string>>(nameof(SmsOtcCodeGeneratorActivity), null);
            if (!otcCodeOperation.Status)
            {
                return Result<VerifyUserSmsOtcResponse>.Failure("OTCCodeGeneration");
            }

            var sendSmsOperation = await SendSmsAsync(context, request.Mobile, otcCodeOperation.Data);
            if (!sendSmsOperation.Status)
            {
                return Result<VerifyUserSmsOtcResponse>.Failure("SendSms");
            }

            var operation = await HandleOtcAsync(context,otcCodeOperation.Data, request);
            return operation;

        }

        private async Task<Result<VerifyUserSmsOtcResponse>> HandleOtcAsync(IDurableOrchestrationContext context, string expectedChallengeCode, SendOtcRequest request)
        {
            using (var timeoutCts = new CancellationTokenSource())
            {
                var timeoutTask = context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(60), timeoutCts.Token);
                var challengeResponseTask = context.WaitForExternalEvent<VerifyUserSmsOtcRequest>("SmsChallengeResponse");

                var winner = await Task.WhenAny(challengeResponseTask, timeoutTask);
                if (winner == challengeResponseTask)
                {
                    timeoutCts.Cancel();
                    var actualChallengeCode = challengeResponseTask.Result.Code;
                    if (string.Equals(expectedChallengeCode, actualChallengeCode))
                    {
                        var updateOperation = await UpdateCustomerAsync(context, request.CustomerId);
                        if (!updateOperation.Status)
                        {
                            return Result<VerifyUserSmsOtcResponse>.Failure("UpdateCustomer");
                        }

                        var response = new VerifyUserSmsOtcResponse
                        {
                            Mobile = request.Mobile,
                            VerifiedAt = context.CurrentUtcDateTime
                        };
                        return Result<VerifyUserSmsOtcResponse>.Success(response);
                    }
                }
                else
                {
                    return Result<VerifyUserSmsOtcResponse>.Failure("OTCTimeout");
                }
            }
            
            return Result<VerifyUserSmsOtcResponse>.Failure("InvalidOTCEntered");
        }

        private async Task<Result> SendSmsAsync(IDurableOrchestrationContext context, string mobile, string code)
        {
            var sendSmsRequest = new SendSmsRequest
            {
                Mobile = mobile,
                Message = $"Your verification code is {code}"
            };

            var operation = await context.CallActivityAsync<Result>(nameof(SendSmsActivity), sendSmsRequest);
            return operation;
        }

        private async Task<Result> UpdateCustomerAsync(IDurableOrchestrationContext context, string customerId)
        {
            var updateCustomerRequest = new UpdateCustomerRequest
            {
                CustomerId = customerId,
                MobileVerified = true
            };
            var operation = await context.CallActivityAsync<Result>(nameof(UpdateCustomerActivity), updateCustomerRequest);
            return operation;
        }
    }
}