using FluentValidation.Results;

namespace Demo.DurableFunctions.Core
{
    public class Result<TData>
    {
        public TData Data { get; set; }

        public string ErrorCode { get; set; }

        public ValidationResult ValidationResult { get; set; }

        public bool Status => ValidationResult == null || ValidationResult.IsValid;

        public static Result<TData> Success(TData data)
        {
            return new Result<TData>
            {
                Data = data
            };
        }

        public static Result<TData> Failure(string errorCode)
        {
            return Failure(errorCode, new ValidationResult(new[]
            {
                new ValidationFailure(errorCode, errorCode)
            }));
        }

        public static Result<TData> Failure(string errorCode, ValidationResult validationResult)
        {
            return new Result<TData>
            {
                ErrorCode = errorCode,
                ValidationResult = validationResult
            };
        }
    }
}