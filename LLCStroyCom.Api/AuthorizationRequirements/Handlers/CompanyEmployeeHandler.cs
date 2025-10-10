using LLCStroyCom.Api.AuthorizationRequirements.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace LLCStroyCom.Api.AuthorizationRequirements.Handlers;

public class CompanyEmployeeHandler : AuthorizationHandler<CompanyEmployeeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CompanyEmployeeRequirement requirement)
    {
        var claimCompanyId = context.User.FindFirst("Company");
        if (claimCompanyId is null || string.IsNullOrEmpty(claimCompanyId.Value))
        {
            context.Fail(new AuthorizationFailureReason(this, "Company claim not found"));
            return Task.CompletedTask;
        }

        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail(new AuthorizationFailureReason(this, "HttpContext not available"));
            return Task.CompletedTask;
        }
        
        var routeCompanyId = httpContext.Request.RouteValues["companyId"]?.ToString();

        bool isMatchCompany = Guid.TryParse(routeCompanyId, out var companyId) &&
                              Guid.TryParse(claimCompanyId.Value, out var userCompanyId) &&
                              companyId == userCompanyId;

        if (!isMatchCompany)
        {
            isMatchCompany = string.Equals(routeCompanyId, claimCompanyId.Value, StringComparison.OrdinalIgnoreCase);
        }

        if (isMatchCompany)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "User is not employee of this company"));
        }

        return Task.CompletedTask;
    }
}