using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Specifications.Projects;

public class CompanySpecification : Specification<Project>
{
    public CompanySpecification(Guid? companyId)
    {
        if (companyId.HasValue)
        {
            Query.Where(p => p.CompanyId == companyId);
        }
    }
}