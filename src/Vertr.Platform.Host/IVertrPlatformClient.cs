using Refit;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host;

public interface IVertrPlatformClient
{
    #region market-data

    [Get("/market-data/subsciptions")]
    public Task<CandleSubscription[]?> GetSubscriptions();

    [Get("/market-data/instruments")]
    public Task<Instrument[]?> GetInstruments();

    [Get("/market-data/instrument-by-ticker/{classCode}/{ticker}")]
    public Task<Instrument?> GetInstrumentByTicker(string classCode, string ticker);

    [Get("/market-data/instrument-by-id/{instrumentId}")]
    public Task<Instrument?> GetInstrumentById(string instrumentId);

    [Get("/market-data/instrument-find/{query}")]
    public Task<Instrument[]?> FindInstrument(string query);

    #endregion

    #region orders

    [Post("/orders/post")]
    public Task<PostOrderResponse?> PostOrder(PostOrderRequest request);

    [Get("/orders/order-state/{accountId}/{orderId}")]
    public Task<OrderState?> GetOrderState(string accountId, string orderId);

    [Delete("/orders/order/{accountId}/{orderId}")]
    public Task<DateTime> CancelOrder(string accountId, string orderId);

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

    #endregion

    #region portfolio

    [Get("/portfolio/{accountId}")]
    public Task<Portfolio?> GetPortfolio(string accountId, Guid? subAccountId = null);

    [Get("/portfolio/sandbox-accounts")]
    public Task<Account[]?> GetSandboxAccounts();

    [Get("/portfolio/accounts")]
    public Task<Account[]?> GetAccounts();

    [Post("/portfolio/sandbox-account")]
    public Task<string> CreateAccount(string accountName);

    [Put("/portfolio/sandbox-account/{accountId}")]
    public Task<Money?> PayIn(string accountId, [Body] Money money);

    [Delete("/portfolio/sandbox-account/{accountId}")]
    public Task CloseAccount(string accountId);

    [Get("/portfolio/gateway-portfolio")]
    public Task<Portfolio?> GetGatewayPortfolio(string accountId);

    [Get("/portfolio/gateway-operations/{accountId}")]
    public Task<TradeOperation[]?> GetGatewayOperations(string accountId);

    #endregion
}
