using Vertr.Platform.Common.Mediator;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Microsoft.Extensions.Options;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal abstract class OrderHandlerBase
{
    private readonly IInstrumentsRepository _instrumentsRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly OrderExecutionSettings _orderExecutionSettings;

    protected string AccountId => _orderExecutionSettings.AccountId;

    protected IMediator Mediator { get; private set; }

    protected OrderHandlerBase(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IInstrumentsRepository instrumentsRepository,
        IOptions<OrderExecutionSettings> options
        )
    {
        Mediator = mediator;
        _portfolioRepository = portfolioRepository;
        _instrumentsRepository = instrumentsRepository;
        _orderExecutionSettings = options.Value;
    }

    protected async Task<long> GetCurrentPositionInLots(
        Guid subAccountId,
        Guid instrumentId)
    {
        var portfolioIdentity = new PortfolioIdentity(AccountId, subAccountId);

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

        var instrument = await _instrumentsRepository.GetById(instrumentId);

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
