using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.Infrastructure.Common.Mediator;
using Vertr.PortfolioManager.Application.Services;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application;

public static class PortfolioManagerRegistrar
{
    public static IServiceCollection AddPortfolioManager(this IServiceCollection services)
    {
        services.RegisterDataChannel<TradeOperation>();
        services.AddHostedService<TradeOperationConsumerService>();
        services.AddMediatorHandlers(typeof(PortfolioManagerRegistrar).Assembly);

        return services;
    }
}
