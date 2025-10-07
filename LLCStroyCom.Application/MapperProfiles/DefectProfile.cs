using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Application.MapperProfiles;

public class DefectProfile : Profile
{
    public DefectProfile()
    {
        CreateMap<Defect, DefectDto>()
            .ForMember(dest => dest.ChiefEngineer,
                opt => opt.MapFrom(src => src.ChiefEngineer))
            .ForMember(dest => dest.Project,
                opt => opt.MapFrom(src => src.Project));
        CreateMap<Defect, DefectPatchDto>().ReverseMap();
    }
}