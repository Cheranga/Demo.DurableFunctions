using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Application.Requests;

namespace Demo.DurableFunctions.Core.Application.Services
{
    public interface ICheckVisaService
    {
        Task<Result> CheckAsync(CheckVisaRequest request);
    }
}