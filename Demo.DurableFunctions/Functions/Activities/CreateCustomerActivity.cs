using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Core.Domain.Models;
using Demo.DurableFunctions.Core.Domain.Requests;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class CreateCustomerActivity
    {
        private readonly IMapper mapper;
        private readonly ICommandHandler<CreateCustomerCommand> commandHandler;

        public CreateCustomerActivity(IMapper mapper, ICommandHandler<CreateCustomerCommand> commandHandler)
        {
            this.mapper = mapper;
            this.commandHandler = commandHandler;
        }
        
        [FunctionName(nameof(CreateCustomerActivity))]
        public async Task<Customer> RegisterCustomerAsync([ActivityTrigger] IDurableActivityContext context)
        {
            var request = context.GetInput<CreateCustomerRequest>();
            var command = mapper.Map<CreateCustomerCommand>(request);

            var operation = await commandHandler.ExecuteAsync(command, new CancellationToken());
            if (!operation.Status)
            {
                // TODO: Resturn `Result<T>`
                return null;
            }

            return new Customer
            {
                Id = command.CustomerId,
                Email = request.CustomerEmail
            };

        }
    }
}