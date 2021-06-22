using FluentValidation.Results;

namespace Demo.DurableFunctions.Core
{
    public class Result
    {
        public string ErrorCode { get; set; }
        public ValidationResult ValidationResult { get; set; }
        public bool Status => ValidationResult == null || ValidationResult.IsValid;

        public static Result Success()
        {
            return new Result();
        }

        public static Result Failure(string errorCode, string errorMessage)
        {
            return Failure(errorCode, new ValidationResult(new[] {new ValidationFailure("", errorMessage)
            {
                ErrorCode = errorCode
            }}));
        }

        public static Result Failure(string errorCode, ValidationResult validationResult)
        {
            return new Result
            {
                ErrorCode = errorCode,
                ValidationResult = validationResult
            };
        }
    }
    
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

        public static Result<TData> Failure(string errorCode, string errorMessage)
        {
            return Failure(errorCode, new ValidationResult(new[]
            {
                new ValidationFailure("", errorMessage)
                {
                    ErrorCode = errorCode
                }
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