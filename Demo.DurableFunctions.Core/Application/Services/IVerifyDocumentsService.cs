using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Application.Requests;

namespace Demo.DurableFunctions.Core.Application.Services
{
    public interface IVerifyDocumentsService
    {
        Task<Result<int>> VerifyAsync(CheckDocumentsRequest request);
    }
}