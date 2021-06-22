using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Domain.Requests;

namespace Demo.DurableFunctions.Core.Domain.Services
{
    public interface ICheckDriverLicenseService
    {
        Task<Result> CheckAsync(CheckDriverLicenseRequest request);
    }
}