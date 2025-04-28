using Microsoft.Extensions.Logging;
using Vertr.Domain.Enums;

namespace Vertr.Domain.Ports;
public interface ITinvestGateway
{
    public Task<IEnumerable<Instrument>> FindInstrument(string query);

    public Task<InstrumentDetails> GetInstrument(string ticker, string classCode);

    public Task<IEnumerable<HistoricCandle>> GetCandles(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit = null);

    public Task<string> CreateSandboxAccount(string name);

    public Task CloseSandboxAccount(string accountId);

    public Task<Money> SandboxPayIn(
        string accountId,
        Money amount);

    public Task<IEnumerable<Account>> GetAccounts();

    public Task<PostOrderResponse> PostOrder(PostOrderRequest orderRequest);

    public Task<DateTime> CancelOrder(
        string accountId,
        string orderId);

    public Task<OrderState> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType);

    public Task<IEnumerable<Operation>> GetOperations(
        string accountId,
        DateTime? from = null,
        DateTime? to = null);

    public Task<IEnumerable<PositionSnapshot>> GetPositions(string accountId);

    public Task<PortfolioSnapshot> GetPortfolio(string accountId);

    public Task SubscribeToOrderTradesStream(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default);

    public Task SubscribeToOrderStateStream(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default);

    public Task SubscribeToMarketDataStream(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default);

    public Task SubscribeToPortfolioStream(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default);

    public Task SubscribeToPositionsStream(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default);
}
