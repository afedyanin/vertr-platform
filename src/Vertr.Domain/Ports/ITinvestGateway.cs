using Vertr.Domain.Enums;

namespace Vertr.Domain.Ports;
public interface ITinvestGateway
{
    Task<IEnumerable<Instrument>> FindInstrument(string query);

    Task<InstrumentDetails> GetInstrument(string ticker, string classCode);

    Task<IEnumerable<HistoricCandle>> GetCandles(
        string instrumentId,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit = null);

    // TODO: Test them all

    Task<string> CreateSandboxAccount(string name);

    Task CloseSandboxAccount(string accountId);

    Task<Money> SandboxPayIn(string accountId, Money amount);

    Task<IEnumerable<Account>> GetAccounts();

    Task<PostOrderResponse> PostOrder(
        string accountId,
        string instrumentId,
        Guid requestId,
        OrderDirection orderDirection,
        OrderType orderType,
        TimeInForceType timeInForceType,
        PriceType priceType,
        decimal price,
        long quantityLots);

    Task<DateTime> CancelOrder(string accountId, string orderId);

    Task<OrderState> GetOrderState(string accountId, string orderId, PriceType priceType);
}
