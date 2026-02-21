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
        // проверить, что Mid цена в стакане вышла за пределы стат. погрешности. Заполнить TradingDirection
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, BaseAssetOrderBookHandler>();

        // расчет теоретической цены набора фьючей
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, FuturePriceCalculationHandler>();

        // сравнение теор. цены с ценами в стаканах по фьючам, генерация торговых сигналов
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, FutureSignalsGenerator>();

        // проверить позицию в портфеле на соответствие сигналу. Создать реквесты на открытие/разворот портфеля
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, PortfolioPositionHandler<OrderBookChangedEvent>>();

        // отправить маркет ордера в гейтвей
        services.AddSingleton<IEventHandler<OrderBookChangedEvent>, OrderExecutionHandler<OrderBookChangedEvent>>();

        services.AddSingleton<IFuturesProcessingPipeline, FuturesProcessingPipeline>();

        return services;
    }
}
