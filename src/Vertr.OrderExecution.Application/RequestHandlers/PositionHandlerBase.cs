using MediatR;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;
internal abstract class PositionHandlerBase
{
    private readonly IStaticMarketDataProvider _marketDataProvider;

    private readonly IPortfolioManager _portfolioManager;

    protected IMediator Mediator { get; private set; }

    protected PositionHandlerBase(
        IMediator mediator,
        IPortfolioManager portfolioManager,
        IStaticMarketDataProvider marketDataProvider
        )
    {
        Mediator = mediator;

        _portfolioManager = portfolioManager;
        _marketDataProvider = marketDataProvider;
    }

    protected async Task<long> GetCurrentPositionInLots(
        PortfolioIdentity portfolioIdentity,
        InstrumentIdentity instrumentIdentity)
    {
        var instrument = await _marketDataProvider.GetInstrument(instrumentIdentity);

        if (instrument == null)
        {
            throw new InvalidOperationException($"Cannot find instrument with Identity={instrumentIdentity}");
        }

        var portfolio = await _portfolioManager.GetPortfolio(portfolioIdentity);

        if (portfolio == null)
        {
            return 0L;
        }

        // TODO: Test me
        var position = portfolio.Positions.SingleOrDefault(p => p.InstrumentIdentity == instrumentIdentity);

        // TODO: Refactor me
        var posQty = position?.Balance ?? 0L;
        var lotSize = instrument.LotSize ?? 1L;

        return (long)(posQty / lotSize);
    }
}
