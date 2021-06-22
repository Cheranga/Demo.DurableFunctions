using System.Threading.Tasks;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application.Requests;
using Demo.DurableFunctions.DataAccess.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{
    public class UpdateCustomerActivity
    {
        [FunctionName(nameof(UpdateCustomerActivity))]
        public async Task<Result> UpdateCustomerAsync([ActivityTrigger]IDurableActivityContext context,
            [Table("%DatabaseConfig:CustomersTable%")]CloudTable table)
        {
            var request = context.GetInput<UpdateCustomerRequest>();

            var customerDataModel = new CustomerDataModel
            {
                PartitionKey = $"ACTIVE_{request.CustomerId}".ToUpper(),
                RowKey = $"{request.CustomerId}".ToUpper(),
                MobileVerified = request.MobileVerified
            };
            
            var tableOperation = TableOperation.InsertOrMerge(customerDataModel);
            var tableResult = await table.ExecuteAsync(tableOperation);

            return Result.Success();
        }
    }
}