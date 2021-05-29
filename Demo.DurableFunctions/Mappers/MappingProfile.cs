using System;
using AutoMapper;
using Demo.DurableFunctions.DataAccess.Models;
using Demo.DurableFunctions.DTO.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Demo.DurableFunctions.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterAccountRequest, CreateBankAccountRequest>();
            CreateMap<RegisterAccountRequest, CreateCustomerRequest>();

            CreateMap<CreateCustomerRequest, CustomerDataModel>()
                .ForMember(x => x.CustomerId, x => x.MapFrom(y=>Guid.NewGuid().ToString("N").ToUpper()))
                .ForMember(x => x.Email, x => x.MapFrom(y => y.CustomerEmail))
                .ForMember(x => x.Name, x => x.MapFrom(y => y.CustomerName))
                .AfterMap((request, model) =>
                {
                    model.PartitionKey = $"ACTIVE_{model.CustomerId}".ToUpper();
                    model.RowKey = $"{model.CustomerId}".ToUpper();
                });

            CreateMap<CreateBankAccountRequest, BankAccountDataModel>()
                .ForMember(x => x.BankAccountId, x => x.MapFrom(y=>Guid.NewGuid().ToString("N").ToUpper()))
                .ForMember(x => x.Amount, x => x.MapFrom(y => y.Deposit))
                .AfterMap((request, model) =>
                {
                    model.PartitionKey = $"{request.CustomerId}".ToUpper();
                    model.RowKey = $"{request.BankAccountType}_{model.BankAccountId}".ToUpper();
                });

            CreateMap<RegisterAccountRequest, CheckVisaRequest>();
            CreateMap<RegisterAccountRequest, CheckDriverLicenseRequest>();
        }
    }
}