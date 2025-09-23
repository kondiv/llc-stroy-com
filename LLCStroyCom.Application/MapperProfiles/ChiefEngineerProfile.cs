using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Application.MapperProfiles;

public class ChiefEngineerProfile : Profile
{
    public ChiefEngineerProfile()
    {
        CreateMap<ApplicationUser, ChiefEngineerDto>();
    }
}