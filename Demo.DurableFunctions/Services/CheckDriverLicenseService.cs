using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;

namespace Demo.DurableFunctions.Services
{
    public class CheckDriverLicenseService : ICheckDriverLicenseService
    {
        public async Task<Result> CheckAsync(CheckDriverLicenseRequest request)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            return Result.Success();
        }
    }
}