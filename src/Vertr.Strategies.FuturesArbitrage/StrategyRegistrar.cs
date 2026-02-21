using Microsoft.Extensions.DependencyInjection;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Handlers;
using Vertr.Strategies.FuturesArbitrage.Abstractions;
using Vertr.Strategies.FuturesArbitrage.EventHandlers;
using Vertr.Strategies.FuturesArbitrage.Services;

namespace Vertr.Strategies.FuturesArbitrage;

public static class StrategyRegistrar
{
    public static IServiceCollection AddFutureArbitrageStrategy(this IServiceCollection services)
    {
        // 10 проверить, что Mid цена в стакане вышла за пределы стат. погрешности. Заполнить TradingDirection
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, BaseAssetOrderBookHandler>();

        // 20 расчет теоретической цены набора фьючей
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, FuturePriceCalculationHandler>();

        // 30 сравнение теор. цены с ценами в стаканах по фьючам, генерация торговых сигналов
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, FutureSignalsGenerator>();

        // 800 проверить позицию в портфеле на соответствие сигналу. Создать реквесты на открытие/разворот портфеля
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, PortfolioPositionHandler<OrderBookChangedEvent>>();

        // 900 отправить маркет ордера в гейтвей
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, OrderExecutionHandler<OrderBookChangedEvent>>();

        // 1010 сохранить евент
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, EventSavingHandler>();

        services.AddSingleton<IFuturesProcessingPipeline, FuturesProcessingPipeline>();

        return services;
    }
}
