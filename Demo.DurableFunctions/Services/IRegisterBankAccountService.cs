using System.Net.Http;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application.Responses;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Services
{
    public interface IRegisterBankAccountService
    {
        Task<Result<RegisterAccountResponse>> RegisterAsync(HttpRequestMessage request, IDurableClient client);
    }
}