using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Application.MapperProfiles;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<Project, ProjectDto>();
    }
}