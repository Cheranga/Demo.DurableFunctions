using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Domain.Requests;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class SendSmsActivity
    {
        [FunctionName(nameof(SendSmsActivity))]
        public async Task<Result> SendSmsOtcAsync([ActivityTrigger]IDurableActivityContext context,
            [TwilioSms(AccountSidSetting = "TwilioSmsConfig:Sid",AuthTokenSetting = "TwilioSmsConfig:AuthToken", From = "%TwilioSmsConfig:From%")]
            IAsyncCollector<CreateMessageOptions> messages)
        {
            var request = context.GetInput<SendSmsRequest>();

            var message = new CreateMessageOptions(new PhoneNumber(request.Mobile))
            {
                Body = request.Message
            };

            await messages.AddAsync(message);
            return Result.Success();
        }
    }
}