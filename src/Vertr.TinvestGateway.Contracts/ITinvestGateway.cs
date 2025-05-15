using Refit;

namespace Vertr.TinvestGateway.Contracts;

public interface ITinvestGateway
{
    [Get("/accounts")]
    public Task<Account[]?> GetAccounts();

    [Get("/accounts/sandbox")]
    public Task<Account[]?> GetSandboxAccounts();

    [Post("/accounts/sandbox")]
    public Task<string> CreateAccount(string accountName);

    [Delete("/accounts/sandbox/{accountId}")]
    public Task CloseAccount(string accountId);

    [Put("/accounts/sandbox/{accountId}")]
    public Task<Money?> PayIn(string accountId, [Body] Money money);

    [Get("/instruments/find")]
    public Task<Instrument[]?> FindInstrument(string query);

    [Get("/instruments")]
    public Task<Instrument?> GetInstrumentByTicker(string ticker, string classCode);

    [Get("/instruments/id/{instumentId}")]
    public Task<Instrument?> GetInstrumentById(string instumentId);

    [Get("/marketdata/candles")]
    public Task<Candle[]?> GetCandles(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit);

    [Get("/operations")]
    public Task<Operation[]?> GetOperations(
        string accountId,
        DateTime? from = null,
        DateTime? to = null);

    [Get("/operations/positions")]
    public Task<PositionsResponse?> GetPositions(string accountId);

    [Get("/operations/portfolio")]
    public Task<PortfolioResponse?> GetPortfolio(string accountId);

    [Post("/orders")]
    public Task<PostOrderResponse?> PostOrder([Body] PostOrderRequest request);

    [Put("/orders/cancel")]
    public Task<DateTime> CancelOrder(string accountId, string orderId);

    [Get("/orders/state")]
    public Task<OrderState?> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType = PriceType.Unspecified);
}
