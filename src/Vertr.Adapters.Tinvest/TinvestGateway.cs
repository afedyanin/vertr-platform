using Tinkoff.InvestApi;
using Vertr.Domain.Ports;
using Vertr.Adapters.Tinvest.Converters;
using Vertr.Domain;
using Microsoft.Extensions.Options;
using DateTime = System.DateTime;
using Google.Protobuf.WellKnownTypes;


namespace Vertr.Adapters.Tinvest;

internal sealed class TinvestGateway : ITinvestGateway
{
    private readonly InvestApiClient _investApiClient;
    private readonly TinvestSettings _investConfiguration;

    public TinvestGateway(
        InvestApiClient investApiClient,
        IOptions<TinvestSettings> options)
    {
        _investApiClient = investApiClient;
        _investConfiguration = options.Value;
    }

    public async Task<IEnumerable<Instrument>> FindInstrument(string query)
    {
        var request = new Tinkoff.InvestApi.V1.FindInstrumentRequest
        {
            Query = query
        };

        var response = await _investApiClient.Instruments.FindInstrumentAsync(request);

        return response.Instruments.Convert();
    }

    public async Task<InstrumentDetails> GetInstrument(string ticker, string classCode)
    {
        var request = new Tinkoff.InvestApi.V1.InstrumentRequest
        {
            ClassCode = classCode,
            Id = ticker,
            IdType = Tinkoff.InvestApi.V1.InstrumentIdType.Ticker,
        };

        var response = await _investApiClient.Instruments.GetInstrumentByAsync(request);

        return response.Instrument.Convert();
    }

    public async Task<IEnumerable<HistoricCandle>> GetCandles(
        string instrumentId,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit)
    {
        var request = new Tinkoff.InvestApi.V1.GetCandlesRequest
        {
            From = Timestamp.FromDateTime(from),
            To = Timestamp.FromDateTime(to),
            InstrumentId = instrumentId,
            Interval = interval.Convert(),
        };

        if (limit.HasValue)
        {
            request.Limit = limit.Value;
        }

        var response = await _investApiClient.MarketData.GetCandlesAsync(request);

        return response.Candles.Convert();
    }

    public async Task<string> CreateSandboxAccount(string name)
    {
        var request = new Tinkoff.InvestApi.V1.OpenSandboxAccountRequest
        {
            Name = name,
        };

        var response = await _investApiClient.Sandbox.OpenSandboxAccountAsync(request);

        return response.AccountId;
    }

    public async Task CloseSandboxAccount(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.CloseSandboxAccountRequest
        {
            AccountId = accountId,
        };

        _ = await _investApiClient.Sandbox.CloseSandboxAccountAsync(request);
    }

    public async Task<Money> SandboxPayIn(string accountId, Money amount)
    {
        var request = new Tinkoff.InvestApi.V1.SandboxPayInRequest
        {
            AccountId = accountId,
            Amount = amount.Convert(),
        };

        var response = await _investApiClient.Sandbox.SandboxPayInAsync(request);

        return response.Balance.Convert();
    }

    public async Task<IEnumerable<Account>> GetAccounts()
    {
        var response = await _investApiClient.Users.GetAccountsAsync();

        return response.Accounts.Convert();
    }

    public async Task<OrderResponse> PostOrder(
        string accountId,
        string instrumentId,
        Guid requestId,
        OrderDirection orderDirection,
        OrderType orderType,
        TimeInForceType timeInForceType,
        PriceType priceType,
        decimal price,
        long quantityLots)
    {
        var request = new Tinkoff.InvestApi.V1.PostOrderRequest
        {
            AccountId = accountId,
            OrderId = requestId.ToString(),
            Direction = orderDirection.Convert(),
            InstrumentId = instrumentId,
            OrderType = orderType.Convert(),
            TimeInForce = timeInForceType.Convert(),
            PriceType = priceType.Convert(),
            Price = price,
            Quantity = quantityLots
        };

        var response = await _investApiClient.Orders.PostOrderAsync(request);

        var orderResponse = new OrderResponse
        {
            OrderId = response.OrderId,
            OrderRequestId = response.OrderRequestId,
            TrackingId = response.ResponseMetadata.TrackingId,
            ServerTime = response.ResponseMetadata.ServerTime.ToDateTime(),
            Status = response.ExecutionReportStatus.Convert(),
            LotsRequested = response.LotsRequested,
            LotsExecuted = response.LotsExecuted,
            InitialOrderPrice = response.InitialOrderPrice,
            ExecutedOrderPrice = response.ExecutedOrderPrice,
            TotalOrderAmount = response.TotalOrderAmount,
            InitialCommission = response.InitialCommission,
            ExecutedCommission = response.ExecutedCommission,
            Direction = response.Direction.Convert(),
            InitialSecurityPrice = response.InitialSecurityPrice,
            OrderType = response.OrderType.Convert(),
            Message = response.Message,
            InstrumentId = response.InstrumentUid
        };

        return orderResponse;
    }

    public async Task<DateTime> CancelOrder(
        string accountId,
        string orderId)
    {
        var cancelOrderRequest = new Tinkoff.InvestApi.V1.CancelOrderRequest
        {
            AccountId = accountId,
            OrderId = orderId,
        };

        var response = await _investApiClient.Orders.CancelOrderAsync(cancelOrderRequest);

        return response.Time.ToDateTime();
    }

    public async Task<OrderState> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType)
    {
        var orderStateRequest = new Tinkoff.InvestApi.V1.GetOrderStateRequest
        {
            AccountId = accountId,
            OrderId = orderId,
            PriceType = priceType.Convert(),
        };

        var response = await _investApiClient.Orders.GetOrderStateAsync(orderStateRequest);

        return new OrderState
        {
            OrderId = response.OrderId,
            // TODO: Implement this
        };
    }
}
