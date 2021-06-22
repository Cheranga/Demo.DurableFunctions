using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core.Application.Requests;
using Demo.DurableFunctions.DataAccess.Models;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{   
    public class CreateBankAccountActivity
    {
        private readonly IMapper mapper;

        public CreateBankAccountActivity(IMapper mapper)
        {
            this.mapper = mapper;
        }
        
        [FunctionName(nameof(CreateBankAccountActivity))]
        public async Task<BankAccountData> CreateBankAccountAsync([ActivityTrigger] IDurableActivityContext context,
            [Table("%DatabaseConfig:BankAccountsTable%")]IAsyncCollector<BankAccountDataModel> bankAccounts)
        {
            var request = context.GetInput<CreateBankAccountRequest>();

            var dataModel = mapper.Map<BankAccountDataModel>(request);

            await bankAccounts.AddAsync(dataModel);

            return new BankAccountData
            {
                Id = dataModel.BankAccountId,
                BankAccountType = request.BankAccountType
            };
        }
    }
}