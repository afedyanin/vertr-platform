using Microsoft.Extensions.Logging;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using Vertr.TinvestGateway.Contracts.Portfolios;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Services;

internal class PortfolioService : IPortfolioService
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IOrderRequestRepository _orderRequestRepository;
    private readonly IInstrumentProvider _instrumentProvider;
    private readonly ILogger<PortfolioService> _logger;

    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

    public PortfolioService(
        IPortfolioRepository portfolioRepository,
        IOrderRequestRepository orderRequestRepository,
        IInstrumentProvider instrumentProvider,
        ILogger<PortfolioService> logger)
    {
        _portfolioRepository = portfolioRepository;
        _instrumentProvider = instrumentProvider;
        _orderRequestRepository = orderRequestRepository;
        _logger = logger;
    }

    public async Task Update(PostOrderResponse orderResponse, Guid portfolioId)
    {
        await _portfolioRepository.BindOrderToPortfolio(orderResponse.OrderId, portfolioId);
        await Semaphore.WaitAsync();

        try
        {
            var builder = await CreateBuilderByPortfolioId(portfolioId);
            var newPortfolio = builder.Apply(orderResponse).Build();
            await _portfolioRepository.Save(newPortfolio);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task Update(OrderTrades orderTrades)
    {
        var portfolioId = await _portfolioRepository.GetPortfolioByOrderId(orderTrades.OrderId);

        if (!portfolioId.HasValue)
        {
            _logger.LogError($"Cannot determine portfolio for orderId={orderTrades.OrderId}. Skipping trades...");
            return;
        }

        await Semaphore.WaitAsync();

        try
        {
            var builder = await CreateBuilderByPortfolioId(portfolioId.Value);
            var newPortfolio = builder.Apply(orderTrades).Build();
            await _portfolioRepository.Save(newPortfolio);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task BindOrderToPortfolio(OrderState orderState)
    {
        if (string.IsNullOrEmpty(orderState.OrderId))
        {
            return;
        }

        if (string.IsNullOrEmpty(orderState.OrderRequestId))
        {
            return;
        }

        var orderRequest = await _orderRequestRepository.Get(new Guid(orderState.OrderRequestId));

        if (orderRequest == null)
        {
            return;
        }

        await _portfolioRepository.BindOrderToPortfolio(orderState.OrderId, orderRequest.PortfolioId);
    }

    private async Task<PortfolioBuilder> CreateBuilderByPortfolioId(Guid portfolioId)
    {
        var portfolio = await _portfolioRepository.GetById(portfolioId);
        var instruments = await _instrumentProvider.GetAll();

        var builder = portfolio == null ? new PortfolioBuilder(portfolioId, instruments, _logger) : new PortfolioBuilder(portfolio, instruments, _logger);

        return builder;
    }

    private class PortfolioBuilder
    {
        private readonly Guid _portfolioId;
        private readonly Dictionary<Guid, Position> _comissions = [];
        private readonly Dictionary<Guid, Position> _positions = [];
        private readonly Dictionary<string, Instrument> _instrumentsByTicker;
        // private readonly ILogger _logger;

        public PortfolioBuilder(
            Portfolio portfolio,
            IEnumerable<Instrument> instruments,
            ILogger logger)
            : this(portfolio.Id, instruments, logger)
        {
            _positions = portfolio.Positions.ToDictionary(x => x.InstrumentId, x => x);
            _comissions = portfolio.Comissions.ToDictionary(x => x.InstrumentId, x => x);
        }
        public PortfolioBuilder(
            Guid portfolioId,
            IEnumerable<Instrument> instruments,
            ILogger logger)
        {
            _portfolioId = portfolioId;
            _instrumentsByTicker = InstrumentsByTicker(instruments);
            // _logger = logger;
        }

        public PortfolioBuilder Apply(PostOrderResponse orderResponse)
        {
            var commision = orderResponse.ExecutedCommission;

            if (commision == null)
            {
                return this;
            }

            _instrumentsByTicker.TryGetValue(commision.Currency, out var instrument);
            var key = instrument == null ? Guid.Empty : instrument.Id;

            _comissions.TryGetValue(key, out var comissionEntry);

            _comissions[key] = new Position
            {
                InstrumentId = key,
                Balance = comissionEntry.Balance + commision.Value,
            };

            return this;
        }

        public PortfolioBuilder Apply(OrderTrades orderTradees)
        {
            var qtySign = orderTradees.Direction == OrderDirection.Sell ? -1 : 1;

            foreach (var trade in orderTradees.Trades)
            {
                ApplyTrade(trade, orderTradees.InstrumentId, qtySign);
            }

            return this;
        }

        public Portfolio Build()
        {
            var portfolio = new Portfolio()
            {
                Id = _portfolioId,
                UpdatedAt = DateTime.UtcNow,
                Comissions = [.. _comissions.Values],
                Positions = [.. _positions.Values],
            };

            return portfolio;
        }

        private void ApplyTrade(Trade trade, Guid instrumentId, int qtySign)
        {
            var price = trade.Price?.Value ?? 0;
            _instrumentsByTicker.TryGetValue(trade.Price?.Currency ?? string.Empty, out var currencyInstrument);
            var currencyId = currencyInstrument?.Id ?? Guid.Empty;

            // _logger.LogInformation($"Applying trade: {trade}. sPrice={trade.Price} CurrencyInstrument={currencyInstrument} CurrencyId={currencyId}");

            _positions.TryGetValue(instrumentId, out var positionEntry);
            _positions.TryGetValue(currencyId, out var moneyPositionEntry);

            _positions[instrumentId] = new Position
            {
                InstrumentId = instrumentId,
                Balance = positionEntry.Balance + trade.Quantity * qtySign
            };

            _positions[currencyId] = new Position
            {
                InstrumentId = currencyId,
                Balance = moneyPositionEntry.Balance + price * trade.Quantity * qtySign * (-1)
            };
        }

        private Dictionary<string, Instrument> InstrumentsByTicker(IEnumerable<Instrument> instruments)
            => instruments.ToDictionary(x => x.Ticker, x => x, StringComparer.OrdinalIgnoreCase);
    }
}