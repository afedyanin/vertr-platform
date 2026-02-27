using System.Diagnostics;
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

        var portfolio = _portfolioRepository.GetByName(signal.PortfolioName);

        if (portfolio == null)
        {
            _logger.LogWarning("Portfolio is not found for name={Name}", signal.PortfolioName);
            return null;
        }

        var instrumentId = signal.Instrument.Id;
        Debug.Assert(instrumentId != Guid.Empty);

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
            var openRequest = new MarketOrderRequest
            {
                RequestId = Guid.NewGuid(),
                TradingSignal = signal.PortfolioName,
                InstrumentId = instrumentId,
                PortfolioId = portfolio.Id,
                QuantityLots = DefaultQtyLots,
                Direction = signal.Direction,
            };

            _logger.LogInformation("Open position Request: QuantityLots={QuantityLots}", openRequest.QuantityLots);
            return openRequest;
        }

        // Reverse position
        var lotSize = signal.Instrument.LotSize ?? 1;
        var positionQtyLots = (long)(position.Amount / lotSize);
        var reverseRequest = new MarketOrderRequest
        {
            RequestId = Guid.NewGuid(),
            TradingSignal = signal.PortfolioName,
            InstrumentId = instrumentId,
            PortfolioId = portfolio.Id,
            QuantityLots = Math.Abs(positionQtyLots) * 2,
            Direction = signal.Direction,
        };

        _logger.LogInformation("Reverse position Request: QuantityLots={QuantityLots}", reverseRequest.QuantityLots);
        return reverseRequest;
    }

    public async Task<IEnumerable<MarketOrderRequest>> CloseAllPositions()
    {
        var portfolios = _portfolioRepository.GetAll();

        var requests = new List<MarketOrderRequest>();

        foreach (var kvp in portfolios)
        {
            foreach (var position in kvp.Value.Positions)
            {
                if (position.Amount == decimal.Zero)
                {
                    continue;
                }

                var instrument = _instrumentRepository.GetById(position.InstrumentId);

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
                var direction = position.Amount > 0 ? TradingDirection.Sell : TradingDirection.Buy;

                var closeRequest = new MarketOrderRequest
                {
                    RequestId = Guid.NewGuid(),
                    TradingSignal = kvp.Key,
                    InstrumentId = position.InstrumentId,
                    PortfolioId = kvp.Value.Id,
                    QuantityLots = Math.Abs(positionQtyLots),
                    Direction = direction,
                };

                closeRequest.OrderId = await _tinvestGateway.PostMarketOrder(closeRequest);
                requests.Add(closeRequest);
            }
        }

        _logger.LogDebug("CloseAllPositions request executed.");
        return requests;
    }
}

