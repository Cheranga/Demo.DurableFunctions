using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.DataAccess.Models;
using Demo.DurableFunctions.DTO.Requests;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Patterns.FunctionChaining
{   
    public class CreateBankAccountActivity
    {
        private readonly IMapper mapper;

        public CreateBankAccountActivity(IMapper mapper)
        {
            this.mapper = mapper;
        }
        
        [FunctionName(nameof(CreateBankAccountActivity))]
        public async Task<BankAccount> CreateBankAccountAsync([ActivityTrigger] IDurableActivityContext context,
            [Table("%DatabaseConfig:BankAccountsTable%")]IAsyncCollector<BankAccountDataModel> bankAccounts)
        {
            var request = context.GetInput<CreateBankAccountRequest>();

            var dataModel = mapper.Map<BankAccountDataModel>(request);

            await bankAccounts.AddAsync(dataModel);

            return new BankAccount
            {
                Id = dataModel.BankAccountId,
                BankAccountType = request.BankAccountType
            };
        }
    }
}