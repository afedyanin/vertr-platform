using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;
using Vertr.Common.DataAccess.Repositories;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Models.Orders;
using Vertr.TinvestGateway.Models.Orders.Enums;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Services;

internal class PortfolioService : IPortfolioService
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IPortfolioOrdersRepository _portfolioOrdersRepository;
    private readonly IOrderRequestRepository _orderRequestRepository;
    private readonly IInstrumentProvider _instrumentProvider;
    private readonly ILogger<PortfolioService> _logger;

    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

    public PortfolioService(
        IPortfolioRepository portfolioRepository,
        IPortfolioOrdersRepository portfolioOrdersRepository,
        IOrderRequestRepository orderRequestRepository,
        IInstrumentProvider instrumentProvider,
        ILogger<PortfolioService> logger)
    {
        _portfolioRepository = portfolioRepository;
        _orderRequestRepository = orderRequestRepository;
        _instrumentProvider = instrumentProvider;
        _portfolioOrdersRepository = portfolioOrdersRepository;
        _logger = logger;
    }

    public async Task Update(PostOrderResponse orderResponse, Guid portfolioId)
    {
        var commission = orderResponse.ExecutedCommission;

        if (commission == null)
        {
            return;
        }

        await _portfolioOrdersRepository.BindOrderToPortfolio(orderResponse.OrderId, portfolioId);
        await Semaphore.WaitAsync();

        try
        {
            var builder = await CreateBuilderByPortfolioId(portfolioId);
            var newPortfolio = builder.ApplyComission(commission.Value, commission.Currency).Build();
            await _portfolioRepository.Save(newPortfolio);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public async Task Update(OrderTrades orderTrades)
    {
        var portfolioId = await _portfolioOrdersRepository.GetPortfolioByOrderId(orderTrades.OrderId);

        if (!portfolioId.HasValue)
        {
            _logger.LogError($"Cannot determine portfolio for orderId={orderTrades.OrderId}. Skipping trades...");
            return;
        }

        await Semaphore.WaitAsync();

        try
        {
            var builder = await CreateBuilderByPortfolioId(portfolioId.Value);
            var direction =
                orderTrades.Direction == OrderDirection.Buy ? TradingDirection.Buy :
                    orderTrades.Direction == OrderDirection.Sell ? TradingDirection.Sell :
                        TradingDirection.Hold;

            var newPortfolio = builder.ApplyTrades(orderTrades.InstrumentId, orderTrades.Trades, direction).Build();
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

        await _portfolioOrdersRepository.BindOrderToPortfolio(orderState.OrderId, orderRequest.PortfolioId);
    }

    private async Task<PortfolioBuilder> CreateBuilderByPortfolioId(Guid portfolioId)
    {
        var portfolio = await _portfolioRepository.GetById(portfolioId);
        var instruments = await _instrumentProvider.GetAll();

        var builder = portfolio == null ? new PortfolioBuilder(portfolioId, instruments) : new PortfolioBuilder(portfolio, instruments);

        return builder;
    }
}