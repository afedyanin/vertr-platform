using Vertr.Platform.Common.Mediator;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Microsoft.Extensions.Options;

namespace Vertr.OrderExecution.Application.CommandHandlers;

internal abstract class OrderHandlerBase
{
    private readonly IInstrumentsRepository _instrumentsRepository;
    private readonly IPortfolioRepository _portfolioProvider;
    private readonly OrderExecutionSettings _orderExecutionSettings;

    protected string AccountId => _orderExecutionSettings.AccountId;

    protected IMediator Mediator { get; private set; }

    protected OrderHandlerBase(
        IMediator mediator,
        IPortfolioRepository portfolioProvider,
        IInstrumentsRepository instrumentsRepository,
        IOptions<OrderExecutionSettings> options)
    {
        Mediator = mediator;
        _portfolioProvider = portfolioProvider;
        _instrumentsRepository = instrumentsRepository;
        _orderExecutionSettings = options.Value;
    }

    // TODO: Use position cache
    protected async Task<long> GetCurrentPositionInLots(
        Guid portfolioId,
        Guid instrumentId)
    {
        var portfolio = await _portfolioProvider.GetById(portfolioId);

        if (portfolio == null)
        {
            throw new InvalidOperationException($"Cannot find potfolio with id={portfolioId}");
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

        var position = portfolio.Positions.FirstOrDefault(p => p.InstrumentId == instrumentId);

        if (position == null)
        {
            return 0L;
        }

        var pos = (long)(position.Balance / instrument.LotSize);

        return pos;
    }
}
