using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Mediator;

namespace Vertr.Backtest.Application;
public static class BacktestRegistrar
{
    public static IServiceCollection AddBacktests(this IServiceCollection services)
    {
        services.AddMediatorHandlers(typeof(BacktestRegistrar).Assembly);
        return services;
    }
}
