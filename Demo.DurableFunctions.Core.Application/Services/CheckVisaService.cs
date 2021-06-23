using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Application.Exceptions;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Core.Domain.Services;

namespace Demo.DurableFunctions.Core.Application.Services
{
    internal class CheckVisaService : ICheckVisaService
    {
        public async Task<Result> CheckAsync(CheckVisaRequest request)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            return Result.Success();
        }
    }
}