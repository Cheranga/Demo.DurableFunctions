using AutoMapper;
using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Infrastructure.DataAccess.Models;

namespace Demo.DurableFunctions.Infrastructure.DataAccess.Mappers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCustomerCommand, CustomerDataModel>().AfterMap((command, model) =>
            {
                model.PartitionKey = $"ACTIVE_{command.CustomerId}".ToUpper();
                model.RowKey = $"{command.CustomerId}".ToUpper();
            });
            
            CreateMap<UpdateCustomerCommand, CustomerDataModel>().AfterMap((command, model) =>
            {
                model.PartitionKey = $"ACTIVE_{command.CustomerId}".ToUpper();
                model.RowKey = $"{command.CustomerId}".ToUpper();
            });
            
            CreateMap<CreateBankAccountCommand, BankAccountDataModel>().AfterMap((command, model) =>
            {
                model.PartitionKey = $"{command.CustomerId}".ToUpper();
                model.RowKey = $"{command.BankAccountType}_{command.BankAccountId}".ToUpper();
            });
        }
    }
}