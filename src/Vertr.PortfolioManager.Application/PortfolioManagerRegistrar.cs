using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Application.CommandHandlers;
using Vertr.PortfolioManager.Application.Repositories;
using Vertr.PortfolioManager.Application.Services;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Commands;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

public static class PortfolioManagerRegistrar
{
    public static IServiceCollection AddPortfolioManager(this IServiceCollection services)
    {
        services.AddOptions<PortfolioSettings>().BindConfiguration(nameof(PortfolioSettings));
        services.AddSingleton<IPortfolioRepository, PortfolioRepository>();

        services.RegisterDataChannel<TradeOperation>();
        services.AddHostedService<TradeOperationConsumerService>();

        services.AddTransient<IRequestHandler<InitialLoadPortfoliosRequest>, InitialLoadPortfoliosHandler>();
        services.AddTransient<IRequestHandler<OverridePositionsRequest>, OverridePositionsHandler>();
        services.AddTransient<IRequestHandler<PayInRequest>, PayInHandler>();

        return services;
    }
}
