using System;
using System.Threading.Tasks;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class CreateCustomerActivity
    {
        [FunctionName(nameof(CreateCustomerActivity))]
        public async Task<Customer> RegisterCustomerAsync([ActivityTrigger] IDurableActivityContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var request = context.GetInput<CreateCustomerRequest>();

            return new Customer
            {
                Id = Guid.NewGuid().ToString("N"),
                Email = request.CustomerEmail
            };
        }
    }
}