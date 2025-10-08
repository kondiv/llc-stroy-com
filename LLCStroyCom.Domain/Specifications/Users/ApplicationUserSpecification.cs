using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Specifications.Users;

public class ApplicationUserSpecification : Specification<ApplicationUser>
{
    public ApplicationUserSpecification(ApplicationUserFilter filter)
    {
        Query.Where(u => u.Name.ToLower().Contains(filter.Name!.ToLower()), !string.IsNullOrEmpty(filter.Name));
        Query.Where(u => u.RoleId == filter.RoleId, filter.RoleId is not null);

        switch (filter.OrderBy)
        {
            case "name":
                if (filter.OrderByDescending)
                {
                    Query.OrderByDescending(u => u.Name);
                }
                else
                {
                    Query.OrderBy(u => u.Name);
                }
                break;
            default:
                Query.OrderBy(u => u.Name);
                break;
        }
    }
}