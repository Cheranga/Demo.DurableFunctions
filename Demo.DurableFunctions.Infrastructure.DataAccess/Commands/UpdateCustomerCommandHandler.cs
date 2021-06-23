using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Infrastructure.DataAccess.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Infrastructure.DataAccess.Commands
{
    public class UpdateCustomerCommandHandler : CommandHandlerBase, ICommandHandler<UpdateCustomerCommand>
    {
        private readonly IMapper mapper;

        public UpdateCustomerCommandHandler(IMapper mapper, ITableStorageFactory tableStorageFactory, ILogger<UpdateCustomerCommandHandler> logger) : base(tableStorageFactory, logger)
        {
            this.mapper = mapper;
        }

        public async Task<Result> ExecuteAsync(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            var customerDataModel = mapper.Map<CustomerDataModel>(command);

            var operation = await ExecuteOperationAsync("Customers", TableOperationType.InsertOrMerge, customerDataModel, cancellationToken);
            if (operation.Status)
            {
                return operation;
            }

            return Result.Failure("UpdateCustomerError", "error occurred when updating the customer");
        }
    }
}