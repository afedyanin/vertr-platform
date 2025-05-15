using MediatR;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Commands;
internal abstract class PositionHandlerBase
{
    protected IMediator Mediator { get; private set; }

    protected IPortfolioClient PortfolioClient { get; private set; }

    protected IStaticMarketDataProvider StaticMarketDataProvider { get; private set; }

    protected PositionHandlerBase(
        IMediator mediator,
        IPortfolioClient portfolioClient,
        IStaticMarketDataProvider staticMarketDataProvider
        )
    {
        Mediator = mediator;
        PortfolioClient = portfolioClient;
        StaticMarketDataProvider = staticMarketDataProvider;
    }

    protected async Task<long> GetCurrentPositionInLots(
        string accountId,
        Guid instrumentId)
    {
        var portfolio = await PortfolioClient.GetLast(accountId);

        if (portfolio == null)
        {
            return 0L;
        }

        var position = portfolio.Positions.SingleOrDefault(p => p.InstrumentId == instrumentId);
        var posQty = position?.Balance ?? 0L;

        return StaticMarketDataProvider.PositionToLots(instrumentId, posQty);
    }
}
