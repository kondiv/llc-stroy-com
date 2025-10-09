using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Specifications.Defects;

public class DefectSpecification : Specification<Defect>
{
    public DefectSpecification(DefectFilter filter)
    {
        Query.Where(d => d.Name.ToLower().Contains(filter.Name!.ToLower()), !string.IsNullOrEmpty(filter.Name));
        Query.Where(d => d.Status.Equals(filter.Status), filter.Status is not null);

        switch (filter.OrderBy)
        {
            case "name":
                if (filter.OrderByDescending)
                {
                    Query.OrderByDescending(d => d.Name);
                }
                else
                {
                    Query.OrderBy(d => d.Name);
                }
                break;
            case "status":
                if (filter.OrderByDescending)
                {
                    Query.OrderByDescending(d => d.Status);
                }
                else
                {
                    Query.OrderBy(d => d.Status);
                }
                break;
            default:
                Query.OrderBy(d => d.Name);
                break;
        }
    }
}