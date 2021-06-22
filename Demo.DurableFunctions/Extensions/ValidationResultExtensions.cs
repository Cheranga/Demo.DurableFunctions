using System.Linq;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Domain.Responses;

namespace Demo.DurableFunctions.Extensions
{
    public static class ValidationResultExtensions
    {
        public static ErrorResponse ToErrorResponse<TData>(this Result<TData> operation)
        {
            if (operation == null || operation.Status)
            {
                return null;
            }

            var errorResponse = new ErrorResponse
            {
                ErrorCode = operation.ErrorCode,
                Errors = operation.ValidationResult.Errors.Select(x => new Error
                {
                    ErrorCode = x.ErrorCode,
                    ErrorMessage = x.ErrorMessage
                }).ToList()
            };

            return errorResponse;
        }
    }
}