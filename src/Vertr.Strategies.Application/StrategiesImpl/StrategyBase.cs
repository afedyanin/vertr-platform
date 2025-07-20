using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Strategies.Application.StrategiesImpl;
internal abstract class StrategyBase
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IMarketDataRepository MarketDataRepository { get; private set; }

    private readonly IMediator _mediator;

    protected StrategyBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        MarketDataRepository = ServiceProvider.GetRequiredService<IMarketDataRepository>();
        _mediator = ServiceProvider.GetRequiredService<IMediator>();
    }

    public Guid Id { get; init; }

    public Guid InstrumentId { get; init; }

    public PortfolioIdentity PortfolioIdentity { get; init; }

    public long QtyLots { get; init; }

    public virtual async Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default)
    {
        var sigal = CreateSignal(candle);
        await _mediator.Send(sigal, cancellationToken);
    }

    public abstract TradingSignalRequest CreateSignal(Candle candle);
}
