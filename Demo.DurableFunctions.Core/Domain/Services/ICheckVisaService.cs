using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Domain.Requests;

namespace Demo.DurableFunctions.Core.Domain.Services
{
    public interface ICheckVisaService
    {
        Task<Result> CheckAsync(CheckVisaRequest request);
    }
}