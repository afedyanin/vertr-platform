using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Vertr.Infrastructure.Common.Jobs.Filters;

internal class SkipAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
