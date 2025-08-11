using Vertr.Platform.Common.Mediator;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal abstract class OrderHandlerBase
{
    private readonly IInstrumentsRepository _marketDataProvider;
    private readonly IPortfolioRepository _portfolioRepository;

    protected IMediator Mediator { get; private set; }

    protected OrderHandlerBase(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IInstrumentsRepository marketDataProvider
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
        var portfolio = _portfolioRepository.GetPortfolio(portfolioIdentity);

        if (portfolio == null)
        {
            return 0L;
        }

        var position = portfolio.Positions.SingleOrDefault(p => p.InstrumentId == instrumentId);

        if (position == null)
        {
            return 0L;
        }

        var instrument = await _marketDataProvider.GetById(instrumentId);

        if (instrument == null)
        {
            throw new InvalidOperationException($"Cannot find instrument with Id={instrumentId}");
        }

        if (instrument.LotSize == null)
        {
            throw new InvalidOperationException($"Cannot determine lot size for instrument with Id={instrumentId}");
        }

        return (long)(position.Balance / instrument.LotSize);
    }
}
