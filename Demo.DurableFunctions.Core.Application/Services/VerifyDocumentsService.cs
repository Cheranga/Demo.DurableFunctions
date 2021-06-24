using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Core.Domain.Services;

namespace Demo.DurableFunctions.Core.Application.Services
{
    internal class VerifyDocumentsService : IVerifyDocumentsService
    {
        public Task<Result<int>> VerifyAsync(CheckDocumentsRequest request)
        {
            var random = new Random();
            var next = random.Next(0, (request.DocumentCount + 1));

            var remainingDocumentCount = request.DocumentCount - next;
            remainingDocumentCount = remainingDocumentCount <= 0 ? 0 : remainingDocumentCount;

            var operation = Result<int>.Success(remainingDocumentCount);
            return Task.FromResult(operation);
        }
    }
}