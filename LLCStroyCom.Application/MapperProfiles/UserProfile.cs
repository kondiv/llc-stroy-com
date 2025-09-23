using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Application.MapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Type))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company));
    }
}