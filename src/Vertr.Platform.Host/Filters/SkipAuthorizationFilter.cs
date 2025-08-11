using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Vertr.Platform.Host.Filters;

public class SkipAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
