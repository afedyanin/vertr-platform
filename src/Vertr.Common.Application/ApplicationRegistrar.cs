using Microsoft.Extensions.DependencyInjection;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.LocalStorage;
using Vertr.Common.Application.Services;

namespace Vertr.Common.Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPortfoliosLocalStorage, PortfoliosLocalStorage>();
        services.AddSingleton<IInstrumentsLocalStorage, InstrumentsLocalStorage>();
        services.AddSingleton<IPortfolioManager, PortfolioManager>();

        services.AddSingleton<CandlesLocalStorage>();
        services.AddSingleton<ICandlesLocalStorage>(sp => sp.GetRequiredService<CandlesLocalStorage>());

        services.AddSingleton<IIndexRatesRepository, IndexRatesLocalStorage>();
        services.AddSingleton<IFutureInfoRepository, FutureInfoLocalStorage>();

        return services;
    }

    public static IServiceCollection AddOrderBookQuoteProvider(this IServiceCollection services)
    {
        services.AddSingleton<OrderBooksLocalStorage>();
        services.AddSingleton<IOrderBooksLocalStorage>(sp => sp.GetRequiredService<OrderBooksLocalStorage>());
        services.AddSingleton<IMarketQuoteProvider>(sp => sp.GetRequiredService<OrderBooksLocalStorage>());
        return services;
    }
}
