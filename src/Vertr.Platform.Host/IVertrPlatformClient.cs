using Refit;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host;

public interface IVertrPlatformClient
{
    [Get("/tinvest/sandbox-accounts")]
    public Task<Account[]?> GetSandboxAccounts();

    [Get("/tinvest/accounts")]
    public Task<Account[]?> GetAccounts();

    [Post("/tinvest/sandbox-account")]
    public Task<string> CreateAccount(string accountName);

    [Put("/tinvest/sandbox-account/{accountId}")]
    public Task<Money?> PayIn(string accountId, [Body] Money money);

    [Delete("/tinvest/sandbox-account/{accountId}")]
    public Task CloseAccount(string accountId);

    [Get("/tinvest/operations/portfolio")]
    public Task<Portfolio?> GetPortfolio(string accountId);

    [Get("/tinvest/instrument-by-ticker/{classCode}/{ticker}")]
    public Task<Instrument?> GetInstrumentByTicker(string classCode, string ticker);

    [Get("/tinvest/instrument-by-id/{instrumentId}")]
    public Task<Instrument?> GetInstrumentById(string instrumentId);

    [Get("/tinvest/instrument-find/{query}")]
    public Task<Instrument[]?> FindInstrument(string query);

    [Get("/tinvest/candles/{classCode}/{ticker}/{interval}")]
    public Task<Candle[]?> GetCandles(
        string classCode,
        string ticker,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit = 100);

    [Post("/tinvest/order")]
    public Task<PostOrderResponse?> PostOrder(PostOrderRequest request);

    [Get("/tinvest/order-state/{accountId}/{orderId}")]
    public Task<OrderState?> GetOrderState(string accountId, string orderId);

    [Delete("/tinvest/order/{accountId}/{orderId}")]
    public Task<DateTime> CancelOrder(string accountId, string orderId);


    [Get("/market-data/subsciptions")]
    public Task<CandleSubscription[]?> GetSubscriptions();


    [Post("/orders/execute")]
    public Task<ExecuteOrderResponse?> ExecuteOrder(ExecuteOrderRequest request);

    [Post("/orders/open")]
    public Task<ExecuteOrderResponse?> OpenPosition(OpenPositionRequest request);

    [Post("/orders/close")]
    public Task<ExecuteOrderResponse?> ClosePosition(ClosePositionRequest request);

    [Post("/orders/reverse")]
    public Task<ExecuteOrderResponse?> RevertPosition(ReversePositionRequest request);

    [Post("/orders/signal")]
    public Task<ExecuteOrderResponse?> PorocessSignal(TradingSignalRequest request);


    [Get("/portfolio/{accountId}")]
    public Task<Portfolio?> GetPortfolioSnapshot(string accountId, Guid? bookId = null);
}
