using System;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.DataAccess.Models;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Exceptions;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{
    public class CreateCustomerActivity
    {
        private readonly IMapper mapper;

        public CreateCustomerActivity(IMapper mapper)
        {
            this.mapper = mapper;
        }
        
        [FunctionName(nameof(CreateCustomerActivity))]
        public async Task<Customer> RegisterCustomerAsync([ActivityTrigger] IDurableActivityContext context,
            [Table("%DatabaseConfig:CustomersTable%")]IAsyncCollector<CustomerDataModel> customers)
        {
            var request = context.GetInput<CreateCustomerRequest>();
            var model = mapper.Map<CustomerDataModel>(request);

            await customers.AddAsync(model);

            return new Customer
            {
                Id = model.CustomerId,
                Email = request.CustomerEmail
            };

        }
    }
}