using Refit;

namespace Vertr.TinvestGateway.Contracts;

public interface ITinvestGateway
{
    [Get("/accounts")]
    Task<Account[]?> GetAccounts();

    [Get("/accounts/sandbox")]
    Task<Account[]?> GetSandboxAccounts();

    [Post("/accounts/sandbox")]
    Task<string> CreateAccount(string accountName);

    [Delete("/accounts/sandbox/{accountId}")]
    Task CloseAccount(string accountId);

    [Put("/accounts/sandbox/{accountId}")]
    Task<Money?> PayIn(string accountId, [Body] Money money);

    [Get("/instruments/find")]
    Task<Instrument[]?> FindInstrument(string query);

    [Get("/instruments")]
    Task<Instrument?> GetInstrumentByTicker(string ticker, string classCode);

    [Get("/instruments/id/{instumentId}")]
    Task<Instrument?> GetInstrumentById(string instumentId);

    [Get("/marketdata/candles")]
    Task<Candle[]?> GetCandles(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit);

    [Get("/operations")]
    Task<Operation[]?> GetOperations(
        string accountId,
        DateTime? from = null,
        DateTime? to = null);

    [Get("/operations/positions")]
    Task<PositionsResponse?> GetPositions();

    [Get("/operations/portfolio")]
    Task<PortfolioResponse?> GetPortfolio();

    [Post("/orders")]
    Task<PostOrderResponse?> PostOrder([Body] PostOrderRequest request);

    [Put("/orders/cancel")]
    Task<DateTime> CancelOrder(string accountId, string orderId);

    [Get("/orders/state")]
    Task<OrderState?> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType = PriceType.Unspecified);
}
