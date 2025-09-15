using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Specifications.Projects;

public class CitySpecification : Specification<Project>
{
    public CitySpecification(string? city)
    {
        if(!string.IsNullOrEmpty(city) && !string.IsNullOrWhiteSpace(city))
        {
            Query.Where(p => p.City == city);
        }
    }
}