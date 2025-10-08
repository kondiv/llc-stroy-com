using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Models.PageTokens;

namespace LLCStroyCom.Domain.Specifications.Projects;

public class ProjectSpecification : Specification<Project>
{
    public ProjectSpecification(Guid companyId, ProjectFilter filter, ProjectPageToken? pageToken, int maxPageSize)
    {
        Query.Where(p => p.CompanyId == companyId);
        Query.Where(p => p.City == filter.City, !string.IsNullOrEmpty(filter.City));
        Query.Where(p => p.Status == filter.Status, filter.Status.HasValue);

        if (pageToken is not null)
        {
            BuildQueryWithPageToken(pageToken, maxPageSize);
        }
        else
        {
            BuildQueryWithProjectFilter(filter, maxPageSize);
        }
    }

    private void BuildQueryWithProjectFilter(ProjectFilter filter, int maxPageSize)
    {
        switch (filter.OrderBy)
        {
            case "name":
                if (filter.Descending)
                    Query.OrderByDescending(p => p.Name).ThenBy(p => p.Id);
                else
                    Query.OrderBy(p => p.Name).ThenBy(p => p.Id);
                break;

            case "created-at":
                if (filter.Descending)
                    Query.OrderByDescending(p => p.CreatedAt);
                else
                    Query.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id);
                break;

            default:
                throw new FormatException("Unsupported OrderBy parameter: " + filter.OrderBy);
        }

        Query.Take(maxPageSize + 1);
    }

    private void BuildQueryWithPageToken(ProjectPageToken pageToken, int maxPageSize)
    {
        switch (pageToken.OrderBy)
        {
            case "name":
                if (pageToken.Descending)
                    Query.OrderByDescending(p => p.Name);
                else
                    Query.OrderBy(p => p.Name).ThenBy(p => p.Id);
                break;

            case "created-at":
                if (pageToken.Descending)
                    Query.OrderByDescending(p => p.CreatedAt);
                else
                    Query.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id);
                break;

            default:
                throw new FormatException("Unsupported OrderBy parameter: " + pageToken.OrderBy);
        }
        
        if (pageToken.OrderBy == "created-at")
        {
            if (pageToken.Descending)
            {
                Query.Where(p =>
                    p.CreatedAt < pageToken.ProjectCreatedAt
                    || (p.CreatedAt == pageToken.ProjectCreatedAt && p.Id.CompareTo(pageToken.ProjectId) > 0)
                );
            }
            else
            {
                Query.Where(p =>
                    p.CreatedAt > pageToken.ProjectCreatedAt
                    || (p.CreatedAt == pageToken.ProjectCreatedAt && p.Id.CompareTo(pageToken.ProjectId) > 0)
                );
            }
        }
        else
        {
            if (pageToken.Descending)
            {
                Query.Where(p =>
                    string.Compare(p.Name, pageToken.ProjectName) < 0
                    || (p.Name == pageToken.ProjectName && p.Id.CompareTo(pageToken.ProjectId) > 0)
                );
            }
            else
            {
                Query.Where(p =>
                    string.Compare(p.Name, pageToken.ProjectName) > 0
                    || (p.Name == pageToken.ProjectName && p.Id.CompareTo(pageToken.ProjectId) > 0)
                );
            }
        }

        Query.Take(maxPageSize + 1);
    }
}
