using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Clients;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal sealed class PortfolioManager : IPortfolioManager
{
    // TODO: Get from settings
    private const int DefaultQtyLots = 10;

    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITinvestGatewayClient _tinvestGateway;
    private readonly ILogger<PortfolioManager> _logger;

    public PortfolioManager(
        IPortfolioRepository portfolioRepository,
        ITinvestGatewayClient tinvestGateway,
        ILogger<PortfolioManager> logger)
    {
        _portfolioRepository = portfolioRepository;
        _tinvestGateway = tinvestGateway;
        _logger = logger;
    }

    public MarketOrderRequest? HandleTradingSignal(TradingSignal signal)
    {
        var portfolio = _portfolioRepository.GetByPredictor(signal.Predictor);

        if (portfolio == null)
        {
            _logger.LogWarning("Portfolio is not found for predictor={Predictor}", signal.Predictor);
            return null;
        }

        var position = portfolio.Positions.FirstOrDefault(p => p.InstrumentId == signal.InstrumentId);

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
                InstrumentId = signal.InstrumentId,
                PortfolioId = portfolio.Id,
                QuantityLots = DefaultQtyLots,
            };

            _logger.LogInformation("Open position Request: QuantityLots={QuantityLots}", openRequest.QuantityLots);
            return openRequest;
        }

        // Reverse position
        var reverseDirection = position.Amount > 0 ? (-2) : 2;
        var reverseRequest = new MarketOrderRequest
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = signal.InstrumentId,
            PortfolioId = portfolio.Id,
            QuantityLots = DefaultQtyLots * reverseDirection,
        };

        _logger.LogInformation("Reverse position Request: QuantityLots={QuantityLots}", reverseRequest.QuantityLots);
        return reverseRequest;
    }

    public async Task CloseAllPositions()
    {
        var instruments = await _tinvestGateway.GetAllInstruments();
        var instrumentDict = instruments.ToDictionary(i => i.Id);

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

                if (!instrumentDict.TryGetValue(position.InstrumentId, out var instrument))
                {
                    _logger.LogWarning("Cannot find position instrument by Id={InstrumentId}", position.InstrumentId);
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
                var closeQtyLots = (long)(position.Amount / lotSize * (-1));

                var closeRequest = new MarketOrderRequest
                {
                    RequestId = Guid.NewGuid(),
                    InstrumentId = position.InstrumentId,
                    PortfolioId = portfolio.Id,
                    QuantityLots = closeQtyLots,
                };

                tasks.Add(_tinvestGateway.PostMarketOrder(closeRequest));
            }
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("CloseAllPositions request executed.");
    }
}

public interface IPortfolioManager
{
    public MarketOrderRequest? HandleTradingSignal(TradingSignal signal);
}
