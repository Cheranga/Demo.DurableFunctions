using Demo.DurableFunctions.Core;
using Microsoft.AspNetCore.Mvc;

namespace Demo.DurableFunctions.ResponseFormatters
{
    public interface IResponseFormatter<TResponse> where TResponse : class
    {
        IActionResult GetResponse(Result<TResponse> operation);
    }
}