using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Specifications.Projects;

public class StatusSpecification : Specification<Project>
{
    public StatusSpecification(Status? status)
    {
        if(status.HasValue)
        {
            Query.Where(p => p.Status == status);
        }
    }
}