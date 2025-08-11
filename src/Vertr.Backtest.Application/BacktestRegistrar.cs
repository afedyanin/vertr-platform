using Microsoft.Extensions.DependencyInjection;

namespace Vertr.Backtest.Application;
public static class BacktestRegistrar
{
    public static IServiceCollection AddBacktests(this IServiceCollection services)
    {
        return services;
    }

}
