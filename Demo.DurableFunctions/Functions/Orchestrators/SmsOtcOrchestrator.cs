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
            
            var operation = await HandleOtcWithAttemptsAsync(context, otcCodeOperation.Data, request);
            return operation;
        }


        private async Task<Result<VerifyUserSmsOtcResponse>> HandleOtcWithAttemptsAsync(IDurableOrchestrationContext context, string expectedChallengeCode, SendOtcRequest request, int numOfAttempts = 3)
        {
            using (var cancellationToken = new CancellationTokenSource())
            {
                var sendSmsOperation = await SendSmsAsync(context, request.Mobile, expectedChallengeCode);
                if (!sendSmsOperation.Status)
                {
                    return Result<VerifyUserSmsOtcResponse>.Failure(sendSmsOperation.ErrorCode, sendSmsOperation.ValidationResult);
                }
                
                var timeoutTask = context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(60), cancellationToken.Token);

                for (var attempts = 1; attempts <= numOfAttempts; attempts++)
                {
                    var challengeResponseTask = context.WaitForExternalEvent<VerifyUserSmsOtcRequest>("SmsChallengeResponse");
                    var winner = await Task.WhenAny(timeoutTask, challengeResponseTask);

                    if (winner == timeoutTask)
                    {
                        return Result<VerifyUserSmsOtcResponse>.Failure("OTCTimeout");
                    }
                    
                    var actualChallengeCode = challengeResponseTask.Result.Code;
                    if (string.Equals(expectedChallengeCode, actualChallengeCode))
                    {
                        cancellationToken.Cancel();

                        await context.CallActivityAsync<Result>(nameof(SendSmsActivity), new SendSmsRequest
                        {
                            Mobile = request.Mobile,
                            Message = "Thank you for your verification"
                        });

                        return Result<VerifyUserSmsOtcResponse>.Success(new VerifyUserSmsOtcResponse
                        {
                            Mobile = request.Mobile,
                            VerifiedAt = context.CurrentUtcDateTime
                        });
                    }

                    if (attempts != numOfAttempts)
                    {
                        var warningOperation = await SendSmsAsync(context, request.Mobile, expectedChallengeCode, $"You have only {numOfAttempts - (attempts)} attempts left. ");
                        if (!warningOperation.Status)
                        {
                            return Result<VerifyUserSmsOtcResponse>.Failure("OTCResendFailure");
                        }
                    }
                }

                cancellationToken.Cancel();
            }

            return Result<VerifyUserSmsOtcResponse>.Failure("MaxOTCAttemptsReached");
        }

        private async Task<Result<VerifyUserSmsOtcResponse>> HandleOtcAsync(IDurableOrchestrationContext context, string expectedChallengeCode, SendOtcRequest request)
        {
            
            
            
            var deadLine = context.CurrentUtcDateTime.AddSeconds(300);
            using (var timeoutCts = new CancellationTokenSource())
            {
                for (var attempts = 1; attempts <= 3; attempts++)
                {
                    var timeoutTask = context.CreateTimer(deadLine, timeoutCts.Token);
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
                        
                        await SendSmsAsync(context, request.Mobile, expectedChallengeCode, $"Incorrect code entered, you have {3 - attempts} attempts left.");
                    }
                    else
                    {
                        return Result<VerifyUserSmsOtcResponse>.Failure("OTCTimeout");
                    }
                }
            }
            
            return Result<VerifyUserSmsOtcResponse>.Failure("InvalidOTCEntered");
        }

        private async Task<Result> SendSmsAsync(IDurableOrchestrationContext context, string mobile, string code, string prefix = "")
        {
            var sendSmsRequest = new SendSmsRequest
            {
                Mobile = mobile,
                Message = $"{prefix}Your verification code is {code}"
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