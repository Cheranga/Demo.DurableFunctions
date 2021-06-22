using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Core.Domain.Requests;
using Demo.DurableFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Functions.Activities
{   
    public class CreateBankAccountActivity
    {
        private readonly IMapper mapper;
        private readonly ICommandHandler<CreateBankAccountCommand> commandHandler;

        public CreateBankAccountActivity(IMapper mapper, ICommandHandler<CreateBankAccountCommand> commandHandler)
        {
            this.mapper = mapper;
            this.commandHandler = commandHandler;
        }
        
        [FunctionName(nameof(CreateBankAccountActivity))]
        public async Task<BankAccountData> CreateBankAccountAsync([ActivityTrigger] IDurableActivityContext context)
        {
            var request = context.GetInput<CreateBankAccountRequest>();

            var command = mapper.Map<CreateBankAccountCommand>(request);

            var operation= await commandHandler.ExecuteAsync(command, new CancellationToken());
            if (!operation.Status)
            {
                // TODO: Return Result<T>
                return null;
            }

            return new BankAccountData
            {
                Id = command.BankAccountId,
                BankAccountType = request.BankAccountType
            };
        }
    }
}