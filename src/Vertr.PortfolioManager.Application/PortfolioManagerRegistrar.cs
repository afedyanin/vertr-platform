using Microsoft.Extensions.DependencyInjection;
using Vertr.Platform.Common;
using Vertr.Platform.Common.Channels;
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

        services.AddScoped<ICommandHandler<InitialLoadPortfoliosCommand>, InitialLoadPortfoliosHandler>();
        services.AddScoped<ICommandHandler<PayInCommand>, PayInHandler>();
        services.AddScoped<ICommandHandler<OverridePositionsCommand>, OverridePositionsHandler>();

        return services;
    }
}
