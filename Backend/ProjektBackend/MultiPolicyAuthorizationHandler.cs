using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class EmployeeSelfOnlyOrAdminRequirement : IAuthorizationRequirement { }
public class EmployerSelfOnlyOrAdminRequirement : IAuthorizationRequirement { }
public class EmployerOnlyOrAdminRequirement : IAuthorizationRequirement { }
public class SelfOnlyOrAdminRequirement : IAuthorizationRequirement { }

public class MultiPolicyAuthorizationHandler : AuthorizationHandler<
    EmployeeSelfOnlyOrAdminRequirement,
    EmployerSelfOnlyOrAdminRequirement,
    EmployerOnlyOrAdminRequirement,
    SelfOnlyOrAdminRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MultiPolicyAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EmployeeSelfOnlyOrAdminRequirement requirement)
    {
        return HandleRequirementAsync(context, "EmployeeSelfOnlyOrAdmin");
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EmployerSelfOnlyOrAdminRequirement requirement)
    {
        return HandleRequirementAsync(context, "EmployerSelfOnlyOrAdmin");
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EmployerOnlyOrAdminRequirement requirement)
    {
        return HandleRequirementAsync(context, "EmployerOnlyOrAdmin");
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SelfOnlyOrAdminRequirement requirement)
    {
        return HandleRequirementAsync(context, "SelfOnlyOrAdmin");
    }

    private Task HandleRequirementAsync(AuthorizationHandlerContext context, string policyName)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var resourceId = httpContext?.Request.RouteValues["id"]?.ToString(); // Assuming the resource ID is in the route

        // Admins can access anything
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(context.PendingRequirements.First());
            return Task.CompletedTask;
        }

        // Handle each policy
        switch (policyName)
        {
            case "EmployeeSelfOnlyOrAdmin":
                if (context.User.IsInRole("Employee") && context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resourceId)
                {
                    context.Succeed(context.PendingRequirements.First());
                }
                break;

            case "EmployerSelfOnlyOrAdmin":
                if (context.User.IsInRole("Employer") && context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resourceId)
                {
                    context.Succeed(context.PendingRequirements.First());
                }
                break;

            case "EmployerOnlyOrAdmin":
                if (context.User.IsInRole("Employer"))
                {
                    context.Succeed(context.PendingRequirements.First());
                }
                break;

            case "SelfOnlyOrAdmin":
                if (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resourceId)
                {
                    context.Succeed(context.PendingRequirements.First());
                }
                break;
        }

        return Task.CompletedTask;
    }
}