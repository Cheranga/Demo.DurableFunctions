using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Infrastructure.DataAccess.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Infrastructure.DataAccess.Commands
{
    public class CreateCustomerCommandHandler : CommandHandlerBase, ICommandHandler<CreateCustomerCommand>
    {
        private readonly IMapper mapper;

        public CreateCustomerCommandHandler(IMapper mapper, ITableStorageFactory tableStorageFactory, ILogger<CommandHandlerBase> logger) : base(tableStorageFactory, logger)
        {
            this.mapper = mapper;
        }

        public async Task<Result> ExecuteAsync(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            var customerDataModel = mapper.Map<CustomerDataModel>(command);

            var operation = await ExecuteOperationAsync("Customers", TableOperationType.InsertOrReplace, customerDataModel, cancellationToken);
            if (operation.Status)
            {
                return operation;
            }

            return Result.Failure("CreateCustomerError", "error occurred when creating the customer");
        }
    }
}