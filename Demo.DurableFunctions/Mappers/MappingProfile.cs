using System;
using AutoMapper;
using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Core.Domain.Requests;

namespace Demo.DurableFunctions.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterAccountRequest, CreateBankAccountRequest>();
            CreateMap<RegisterAccountRequest, CreateCustomerRequest>();
            CreateMap<RegisterAccountRequest, CheckVisaRequest>();
            CreateMap<RegisterAccountRequest, CheckDriverLicenseRequest>();
            
            CreateMap<UpdateCustomerRequest, UpdateCustomerCommand>();
            MapToCreateCustomerCommandFromCreateCustomerRequest();
            MapToUpdateBankAccountCommandFromCreateBankAccountRequest();
        }

        private void MapToUpdateBankAccountCommandFromCreateBankAccountRequest()
        {
            CreateMap<CreateBankAccountRequest, CreateBankAccountCommand>()
                .ForMember(x => x.BankAccountId, x => x.MapFrom(y => Guid.NewGuid().ToString("N").ToUpper()))
                .ForMember(x => x.Amount, x => x.MapFrom(y => y.Deposit));

        }

        private void MapToCreateCustomerCommandFromCreateCustomerRequest()
        {
            CreateMap<CreateCustomerRequest, CreateCustomerCommand>()
                .ForMember(x => x.CustomerId, x => x.MapFrom(y => Guid.NewGuid().ToString("N").ToUpper()))
                .ForMember(x => x.Email, x => x.MapFrom(y => y.CustomerEmail))
                .ForMember(x => x.Name, x => x.MapFrom(y => y.CustomerName));
        }
    }
}