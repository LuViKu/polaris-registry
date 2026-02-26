using Hangfire.Dashboard;

namespace OphthalmicRegistry.API.Infrastructure;

/// <summary>Restricts the Hangfire dashboard to users holding the SystemAdmin role.</summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
               && httpContext.User.IsInRole("SystemAdmin");
    }
}
