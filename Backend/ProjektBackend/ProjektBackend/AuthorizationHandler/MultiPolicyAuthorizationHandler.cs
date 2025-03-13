using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class EmployeeSelfOnlyOrAdminRequirement : IAuthorizationRequirement { }
public class EmployerSelfOnlyOrAdminRequirement : IAuthorizationRequirement { }
public class EmployerOnlyOrAdminRequirement : IAuthorizationRequirement { }
public class SelfOnlyOrAdminRequirement : IAuthorizationRequirement { }

public class MultiPolicyAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            switch (requirement)
            {
                case EmployeeSelfOnlyOrAdminRequirement:
                    HandleEmployeeRequirement(context, requirement);
                    break;

                case EmployerSelfOnlyOrAdminRequirement:
                    HandleEmployerSelfRequirement(context, requirement);
                    break;

                case EmployerOnlyOrAdminRequirement:
                    HandleEmployerRequirement(context, requirement);
                    break;

                case SelfOnlyOrAdminRequirement:
                    HandleSelfRequirement(context, requirement);
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private void HandleEmployeeRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (context.User.IsInRole("Admin") ||
            (context.User.IsInRole("Employee") &&
             context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == GetResourceId(context, "UserID")))
        {
            context.Succeed(requirement);
        }
    }

    private void HandleEmployerSelfRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        var resourceUserId = GetResourceId(context, "UserId");
        var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(resourceUserId))
        {
            context.Fail();
            return;
        }

        if (context.User.IsInRole("Admin") || currentUserId == resourceUserId)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private void HandleEmployerRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (context.User.IsInRole("Admin") || context.User.IsInRole("Employer"))
        {
            context.Succeed(requirement);
        }
    }

    private void HandleSelfRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        var resourceUserId = GetResourceId(context, "UserId");
        var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(resourceUserId) || string.IsNullOrEmpty(currentUserId))
        {
            context.Fail();
            return;
        }

        if (int.TryParse(resourceUserId, out int resourceId) &&
            int.TryParse(currentUserId, out int userId))
        {
            if (context.User.IsInRole("Admin") || userId == resourceId)
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }


    private string GetResourceId(AuthorizationHandlerContext context, string paramName)
    {
        if (context.Resource is HttpContext httpContext)
    {
        var value = httpContext.GetRouteValue(paramName)?.ToString();
        
        return value ?? httpContext.Request.Query[paramName].ToString();
    }
    return null;
    }
}
