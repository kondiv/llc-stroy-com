using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Application.MapperProfiles;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, CompanyDto>();
    }
}