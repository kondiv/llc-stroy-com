using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Specifications.Projects;

public class NameSpecification : Specification<Project>
{
    public NameSpecification(string? name)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
        {
            Query.Where(p => p.Name == name);
        }
    }
}