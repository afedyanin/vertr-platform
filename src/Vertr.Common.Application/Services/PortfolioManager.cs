using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal sealed class PortfolioManager : IPortfolioManager
{
    // TODO: Get from settings
    private const int DefaultQtyLots = 10;

    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly IInstrumentsLocalStorage _instrumentRepository;
    private readonly ITradingGateway _tinvestGateway;
    private readonly ILogger<PortfolioManager> _logger;

    public PortfolioManager(
        IPortfoliosLocalStorage portfolioRepository,
        IInstrumentsLocalStorage instrumentRepository,
        ITradingGateway tinvestGateway,
        ILogger<PortfolioManager> logger)
    {
        _portfolioRepository = portfolioRepository;
        _instrumentRepository = instrumentRepository;
        _tinvestGateway = tinvestGateway;
        _logger = logger;
    }

    public MarketOrderRequest? HandleTradingSignal(TradingSignal signal)
    {
        if (signal.Direction == TradingDirection.Hold)
        {
            _logger.LogWarning("Cannot create order request for TradingDirection.Hold");
            return null;
        }

        var portfolio = _portfolioRepository.GetByPredictor(signal.Predictor);

        if (portfolio == null)
        {
            _logger.LogWarning("Portfolio is not found for predictor={Predictor}", signal.Predictor);
            return null;
        }

        var instrumentId = signal.Instrument.Id;

        var position = portfolio.Positions.FirstOrDefault(p => p.InstrumentId == instrumentId);

        if (position.Amount > 0 && signal.Direction == TradingDirection.Buy)
        {
            return null;
        }

        if (position.Amount < 0 && signal.Direction == TradingDirection.Sell)
        {
            return null;
        }

        // Open position
        if (position.Amount == default)
        {
            var openDirection = signal.Direction == TradingDirection.Buy ? 1 : -1;

            var openRequest = new MarketOrderRequest
            {
                RequestId = Guid.NewGuid(),
                Predictor = signal.Predictor,
                InstrumentId = instrumentId,
                PortfolioId = portfolio.Id,
                QuantityLots = DefaultQtyLots * openDirection,
            };

            _logger.LogDebug("Open position Request: QuantityLots={QuantityLots}", openRequest.QuantityLots);
            return openRequest;
        }

        // Reverse position
        var lotSize = signal.Instrument.LotSize ?? 1;
        var positionQtyLots = (long)(position.Amount / lotSize);
        var reverseRequest = new MarketOrderRequest
        {
            RequestId = Guid.NewGuid(),
            Predictor = signal.Predictor,
            InstrumentId = instrumentId,
            PortfolioId = portfolio.Id,
            QuantityLots = positionQtyLots * (-2),
        };

        _logger.LogDebug("Reverse position Request: QuantityLots={QuantityLots}", reverseRequest.QuantityLots);
        return reverseRequest;
    }

    public async Task CloseAllPositions()
    {

        var portfolios = _portfolioRepository.GetAll();

        var tasks = new List<Task>();

        foreach (var portfolio in portfolios)
        {
            foreach (var position in portfolio.Positions)
            {
                if (position.Amount == decimal.Zero)
                {
                    continue;
                }

                var instrument = await _instrumentRepository.GetById(position.InstrumentId);

                if (instrument == null)
                {
                    _logger.LogWarning("Instrument is not found InstrumentId={InstrumentId}", position.InstrumentId);
                    continue;
                }

                if (string.IsNullOrEmpty(instrument.InstrumentType) ||
                    instrument.InstrumentType.Equals("currency", StringComparison.OrdinalIgnoreCase))
                {
                    // Skip currency positions
                    continue;
                }

                // Close position request
                var lotSize = instrument.LotSize ?? 1;
                var positionQtyLots = (long)(position.Amount / lotSize);

                var closeRequest = new MarketOrderRequest
                {
                    RequestId = Guid.NewGuid(),
                    Predictor = portfolio.Predictor ?? string.Empty,
                    InstrumentId = position.InstrumentId,
                    PortfolioId = portfolio.Id,
                    QuantityLots = positionQtyLots * (-1),
                };

                tasks.Add(_tinvestGateway.PostMarketOrder(closeRequest));
            }
        }

        await Task.WhenAll(tasks);
        _logger.LogDebug("CloseAllPositions request executed.");
    }
}

