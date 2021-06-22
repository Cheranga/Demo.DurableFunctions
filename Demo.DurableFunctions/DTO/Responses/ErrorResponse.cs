using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace Demo.DurableFunctions.DTO.Responses
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}