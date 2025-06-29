using Microsoft.Extensions.DependencyInjection;
using Vertr.PortfolioManager.Application.Repositories;
using Vertr.PortfolioManager.Application.Services;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application;

public static class PortfolioManagerRegistrar
{
    public static IServiceCollection AddPortfolioManager(this IServiceCollection services)
    {
        services.AddOptions<PortfolioSettings>().BindConfiguration(nameof(PortfolioSettings));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PortfolioManagerRegistrar).Assembly));
        services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<ITradeOperationService, TradeOperationService>();

        return services;
    }
}
