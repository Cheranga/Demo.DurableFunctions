using AutoMapper;
using Demo.DurableFunctions.DTO.Requests;

namespace Demo.DurableFunctions.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterAccountRequest, CreateBankAccountRequest>();
            CreateMap<RegisterAccountRequest, CreateCustomerRequest>();
        }
    }
}