using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

#pragma warning disable CS8602
#pragma warning disable CS8603
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
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (context.User.IsInRole("Employee"))
        {
            var routeUserId = GetRouteUserId(context);
            var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(routeUserId) || routeUserId == currentUserId)
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }

    private void HandleEmployerSelfRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (context.User.IsInRole("Employer"))
        {
            var routeUserId = GetRouteUserId(context);
            var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(routeUserId) || routeUserId == currentUserId)
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }

    private void HandleEmployerRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        if (context.User.IsInRole("Admin") || context.User.IsInRole("Employer"))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private void HandleSelfRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var routeUserId = GetRouteUserId(context);
        var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(currentUserId))
        {
            context.Fail();
            return;
        }

        if (string.IsNullOrEmpty(routeUserId) || routeUserId == currentUserId)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }

    private string GetRouteUserId(AuthorizationHandlerContext context)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var routeValue = httpContext.GetRouteValue("userId")?.ToString();
            if (!string.IsNullOrEmpty(routeValue))
            {
                return routeValue;
            }

            routeValue = httpContext.GetRouteValue("UserID")?.ToString();
            if (!string.IsNullOrEmpty(routeValue))
            {
                return routeValue;
            }

            var queryValue = httpContext.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(queryValue))
            {
                return queryValue;
            }

            queryValue = httpContext.Request.Query["UserID"].ToString();
            if (!string.IsNullOrEmpty(queryValue))
            {
                return queryValue;
            }
        }

        return null;
    }
}
