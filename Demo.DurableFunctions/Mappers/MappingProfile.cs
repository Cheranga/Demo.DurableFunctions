using System;
using AutoMapper;
using Demo.DurableFunctions.DataAccess.Models;
using Demo.DurableFunctions.DTO.Requests;

namespace Demo.DurableFunctions.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterAccountRequest, CreateBankAccountRequest>();
            CreateMap<RegisterAccountRequest, CreateCustomerRequest>();
            CreateMap<CreateBankAccountRequest, BankAccountDataModel>()
                .ForMember(x => x.Amount, x => x.MapFrom(y => y.Deposit))
                .AfterMap((request, model) =>
                {
                    var bankAccountId = Guid.NewGuid().ToString("N");

                    model.PartitionKey = $"{request.CustomerId}".ToUpper();
                    model.RowKey = $"{request.BankAccountType}_{bankAccountId}".ToUpper();
                    model.BankAccountId = bankAccountId;
                });
        }
    }
}