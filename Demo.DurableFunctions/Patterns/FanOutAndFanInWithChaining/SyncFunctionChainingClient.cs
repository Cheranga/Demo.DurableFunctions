using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Domain.Responses;
using Demo.DurableFunctions.ResponseFormatters;
using Demo.DurableFunctions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Patterns.FanOutAndFanInWithChaining
{
    public class SyncFunctionChainingClient
    {
        private readonly IRegisterBankAccountService registerBankAccountService;
        private readonly IResponseFormatter<RegisterAccountResponse> responseFormatter;

        public SyncFunctionChainingClient(IRegisterBankAccountService registerBankAccountService, IResponseFormatter<RegisterAccountResponse> responseFormatter)
        {
            this.registerBankAccountService = registerBankAccountService;
            this.responseFormatter = responseFormatter;
        }

        [FunctionName(nameof(SyncFunctionChainingClient))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chaining/sync")]
            HttpRequestMessage request,
            [DurableClient]IDurableClient client)
        {
            var operation = await registerBankAccountService.RegisterAsync(request, client);
            var response = responseFormatter.GetResponse(operation);
            return response;
        }
    }
}