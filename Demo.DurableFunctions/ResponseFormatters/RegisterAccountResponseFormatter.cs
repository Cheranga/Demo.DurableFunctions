using System.Net;
using System.Web.Http;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Demo.DurableFunctions.ResponseFormatters
{
    public class RegisterAccountResponseFormatter : IResponseFormatter<RegisterAccountResponse>
    {
        public IActionResult GetResponse(Result<RegisterAccountResponse> operation)
        {
            if (operation == null)
            {
                return new InternalServerErrorResult();
            }

            if (operation.Status)
            {
                return new OkObjectResult(operation.Data);
            }

            return GetErrorResponse(operation);
        }

        private IActionResult GetErrorResponse(Result<RegisterAccountResponse> operation)
        {
            var errorCode = operation.ErrorCode;

            switch (errorCode)
            {
                case "Timeout":
                    return new ObjectResult("Timeout occured")
                    {
                        StatusCode = (int) (HttpStatusCode.InternalServerError)
                    };
                case "InvalidRequest":
                    return new BadRequestObjectResult(operation.ValidationResult);
                case "CreateCustomerError":
                    return new ObjectResult("Error occured when creating the customer")
                    {
                        StatusCode = (int) (HttpStatusCode.InternalServerError)
                    };
                case "CreateBankAccountError":
                    return new ObjectResult("Error occured when creating the bank account")
                    {
                        StatusCode = (int) (HttpStatusCode.InternalServerError)
                    };
                case "InvalidVisa":
                    return new BadRequestObjectResult("According to your VISA this operation cannot proceed");
                case "InvalidDriverLicense":
                    return new BadRequestObjectResult("According to your driver license status this operation cannot proceed");
                default:
                    return new InternalServerErrorResult();
            }
        }
    }
}