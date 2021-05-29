using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Requests;

namespace Demo.DurableFunctions.Services
{
    public interface ICheckDriverLicenseService
    {
        Task<Result> CheckAsync(CheckDriverLicenseRequest request);
    }
}