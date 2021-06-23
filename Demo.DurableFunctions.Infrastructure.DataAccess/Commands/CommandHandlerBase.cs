using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Domain;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Demo.DurableFunctions.Infrastructure.DataAccess.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger<CommandHandlerBase> logger;
        private readonly ITableStorageFactory tableStorageFactory;

        protected CommandHandlerBase(ITableStorageFactory tableStorageFactory, ILogger<CommandHandlerBase> logger)
        {
            this.tableStorageFactory = tableStorageFactory;
            this.logger = logger;
        }

        protected virtual async Task<Result> ExecuteOperationAsync<TData>(string tableName, TableOperationType tableOperationType, TData data, CancellationToken cancellationToken) where TData : ITableEntity
        {
            try
            {
                var table = await tableStorageFactory.GetTableAsync(tableName);
                var tableOperation = tableOperationType == TableOperationType.InsertOrMerge ? TableOperation.InsertOrMerge(data) : TableOperation.InsertOrReplace(data);
                var tableOperationResult = await table.ExecuteAsync(tableOperation, cancellationToken);

                if (tableOperationResult.HttpStatusCode == (int) HttpStatusCode.NoContent)
                {
                    return Result.Success();
                }

                return Result.Failure("CommandError", $"error occurred when executing the command: {tableOperationResult.HttpStatusCode}");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "error occured when executing the data access operation");
            }

            return Result.Failure("DataAccessError", "error occurred when executing data access operation");
        }
    }
}