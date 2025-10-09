using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Specifications.Companies;

public class CompanySpecification : Specification<Company>
{
    public CompanySpecification(CompanyFilter filter)
    {
        Query.Where(c => c.Name.ToLower().Contains(filter.Name!.ToLower()), !string.IsNullOrEmpty(filter.Name));

        switch (filter.OrderBy.ToLower())
        {
            case "name":
                if(filter.OrderByDescending)
                {
                    Query.OrderByDescending(c => c.Name);
                }
                else
                {
                    Query.OrderBy(c => c.Name);
                }
                break;
            default:
                Query.OrderBy(c => c.Name);
                break;
        }
    }
}