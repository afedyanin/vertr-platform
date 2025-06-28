using MediatR;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.RequestHandlers;
internal abstract class PositionHandlerBase
{
    private readonly IStaticMarketDataProvider _marketDataProvider;
    private readonly IPortfolioRepository _portfolioRepository;

    protected IMediator Mediator { get; private set; }

    protected PositionHandlerBase(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider marketDataProvider
        )
    {
        Mediator = mediator;

        _portfolioRepository = portfolioRepository;
        _marketDataProvider = marketDataProvider;
    }

    protected async Task<long> GetCurrentPositionInLots(
        PortfolioIdentity portfolioIdentity,
        Guid instrumentId)
    {
        var instrument = await _marketDataProvider.GetInstrumentById(instrumentId);

        if (instrument == null)
        {
            throw new InvalidOperationException($"Cannot find instrument with Id={instrumentId}");
        }

        var portfolio = _portfolioRepository.GetPortfolio(portfolioIdentity);

        if (portfolio == null)
        {
            return 0L;
        }

        // TODO: Test me
        var position = portfolio.Positions.SingleOrDefault(p => p.InstrumentId == instrumentId);

        // TODO: Refactor me
        var posQty = position?.Balance ?? 0L;
        var lotSize = instrument.LotSize ?? 1L;

        return (long)(posQty / lotSize);
    }
}
