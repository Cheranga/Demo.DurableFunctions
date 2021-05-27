namespace Demo.DurableFunctions.Core
{
    public class Result<TData>
    {
        public TData Data { get; set; }

        public string Error { get; set; }

        public bool Status => string.IsNullOrEmpty(Error);

        public static Result<TData> Success(TData data)
        {
            return new Result<TData>
            {
                Data = data
            };
        }

        public static Result<TData> Failure(string error)
        {
            return new Result<TData>
            {
                Error = error
            };
        }
    }
}