using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Application.Requests;

namespace Demo.DurableFunctions.Core.Application.Services
{
    public class CheckVisaService : ICheckVisaService
    {
        public async Task<Result> CheckAsync(CheckVisaRequest request)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            return Result.Success();
        }
    }
}