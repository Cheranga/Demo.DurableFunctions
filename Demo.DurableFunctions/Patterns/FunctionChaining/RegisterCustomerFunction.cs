using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class RegisterCustomerFunction
    {
        [FunctionName(nameof(RegisterCustomerFunction))]
        public async Task<EssentialCustomerData> RegisterCustomerAsync([ActivityTrigger] IDurableActivityContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            var registerCustomerRequest = context.GetInput<RegisterCustomerRequest>();
            throw new RegisterCustomerException("Blah!");
        }
    }
}