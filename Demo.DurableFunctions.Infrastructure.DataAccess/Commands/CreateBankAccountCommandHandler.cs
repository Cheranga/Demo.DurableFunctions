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
    public class CreateBankAccountCommandHandler : CommandHandlerBase, ICommandHandler<CreateBankAccountCommand>
    {
        private readonly IMapper mapper;

        public CreateBankAccountCommandHandler(IMapper mapper, ITableStorageFactory tableStorageFactory, ILogger<CreateBankAccountCommandHandler> logger) : base(tableStorageFactory, logger)
        {
            this.mapper = mapper;
        }

        public async Task<Result> ExecuteAsync(CreateBankAccountCommand command, CancellationToken cancellationToken)
        {
            var dataModel = mapper.Map<BankAccountDataModel>(command);

            var operation = await ExecuteOperationAsync("BankAccounts", TableOperationType.InsertOrMerge, dataModel, cancellationToken);
            if (operation.Status)
            {
                return operation;
            }

            return Result.Failure("CreateBankAccountError", "error occurred when creating the bank account");
        }
    }
}