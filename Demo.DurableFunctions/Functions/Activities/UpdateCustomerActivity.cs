using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Core.Domain.Requests;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class UpdateCustomerActivity
    {
        private readonly ICommandHandler<UpdateCustomerCommand> commandHandler;
        private readonly IMapper mapper;

        public UpdateCustomerActivity(IMapper mapper, ICommandHandler<UpdateCustomerCommand> commandHandler)
        {
            this.mapper = mapper;
            this.commandHandler = commandHandler;
        }

        [FunctionName(nameof(UpdateCustomerActivity))]
        public async Task<Result> UpdateCustomerAsync([ActivityTrigger] IDurableActivityContext context)
        {
            var request = context.GetInput<UpdateCustomerRequest>();
            var command = mapper.Map<UpdateCustomerCommand>(request);

            // var customerDataModel = new CustomerDataModel
            // {
            //     PartitionKey = $"ACTIVE_{request.CustomerId}".ToUpper(),
            //     RowKey = $"{request.CustomerId}".ToUpper(),
            //     MobileVerified = request.MobileVerified
            // };

            var operation = await commandHandler.ExecuteAsync(command, new CancellationToken());
            return operation;
        }
    }
}