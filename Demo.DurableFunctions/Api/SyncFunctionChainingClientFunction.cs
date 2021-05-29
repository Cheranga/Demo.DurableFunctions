using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Demo.DurableFunctions.Api
{
    public class SyncFunctionChainingClientFunction
    {
        private readonly IHttpRequestBodyReader requestBodyReader;
        private readonly IValidator<RegisterAccountRequest> validator;
        private readonly IRegisterBankAccountService registerBankAccountService;

        public SyncFunctionChainingClientFunction(IRegisterBankAccountService registerBankAccountService)
        {
            this.registerBankAccountService = registerBankAccountService;
        }

        [FunctionName(nameof(SyncFunctionChainingClientFunction))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chaining/sync")]
            HttpRequestMessage request,
            [DurableClient]IDurableClient client)
        {
            var response = await registerBankAccountService.RegisterAsync(request, client);

            return new OkResult();
        }
    }
}