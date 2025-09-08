using Microsoft.Extensions.DependencyInjection;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Infrastructure.Common.Mediator;

namespace Vertr.Backtest.Application;
public static class BacktestRegistrar
{
    public static IServiceCollection AddBacktests(this IServiceCollection services)
    {
        services.AddMediatorHandlers(typeof(BacktestRegistrar).Assembly);

        services.AddSingleton<BacktestProgressSubject>();
        services.AddSingleton<IBacktestObservable>(x => x.GetRequiredService<BacktestProgressSubject>());
        services.AddSingleton<IBacktestProgressHandler>(x => x.GetRequiredService<BacktestProgressSubject>());

        return services;
    }
}
