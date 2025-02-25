using Tinkoff.InvestApi;
using Vertr.Domain.Ports;
using Vertr.Domain;
using Microsoft.Extensions.Options;
using DateTime = System.DateTime;
using Google.Protobuf.WellKnownTypes;
using AutoMapper;
using Google.Protobuf.Collections;
using Vertr.Domain.Enums;


namespace Vertr.Adapters.Tinvest;

internal sealed class TinvestGateway : ITinvestGateway
{
    private readonly IMapper _mapper;
    private readonly InvestApiClient _investApiClient;
    private readonly TinvestSettings _investConfiguration;

    public TinvestGateway(
        IMapper mapper,
        InvestApiClient investApiClient,
        IOptions<TinvestSettings> options)
    {
        _mapper = mapper;
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

        return ToDomain(response.Instruments);
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
        var details = _mapper.Map<InstrumentDetails>(response.Instrument);

        return details;
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
            Interval = _mapper.Map<Tinkoff.InvestApi.V1.CandleInterval>(interval),
        };

        if (limit.HasValue)
        {
            request.Limit = limit.Value;
        }

        var response = await _investApiClient.MarketData.GetCandlesAsync(request);
        var candles = ToDomain(response.Candles);

        return candles;
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

        var resp = await _investApiClient.Sandbox.CloseSandboxAccountAsync(request);
    }

    public async Task<Money> SandboxPayIn(string accountId, Money amount)
    {
        var request = new Tinkoff.InvestApi.V1.SandboxPayInRequest
        {
            AccountId = accountId,
            Amount = _mapper.Map<Tinkoff.InvestApi.V1.MoneyValue>(amount),
        };

        var response = await _investApiClient.Sandbox.SandboxPayInAsync(request);
        var balance = _mapper.Map<Money>(response.Balance);

        return balance;
    }

    public async Task<IEnumerable<Account>> GetAccounts()
    {
        var response = await _investApiClient.Users.GetAccountsAsync();
        var accounts = ToDomain(response.Accounts);

        return accounts;
    }

    public async Task<PostOrderResponse> PostOrder(
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
            Direction = _mapper.Map<Tinkoff.InvestApi.V1.OrderDirection>(orderDirection),
            InstrumentId = instrumentId,
            OrderType = _mapper.Map<Tinkoff.InvestApi.V1.OrderType>(orderType),
            TimeInForce = _mapper.Map<Tinkoff.InvestApi.V1.TimeInForceType>(timeInForceType),
            PriceType = _mapper.Map<Tinkoff.InvestApi.V1.PriceType>(priceType),
            Price = price,
            Quantity = quantityLots
        };

        var response = await _investApiClient.Orders.PostOrderAsync(request);
        var orderResponse = _mapper.Map<PostOrderResponse>(response);

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
            PriceType = _mapper.Map<Tinkoff.InvestApi.V1.PriceType>(priceType),
        };

        var response = await _investApiClient.Orders.GetOrderStateAsync(orderStateRequest);

        var state = _mapper.Map<OrderState>(response);

        return state;
    }

    private IEnumerable<Instrument> ToDomain(RepeatedField<Tinkoff.InvestApi.V1.InstrumentShort> instruments)
    {
        foreach (var instrument in instruments)
        {
            yield return _mapper.Map<Instrument>(instrument);
        }
    }

    private IEnumerable<Account> ToDomain(RepeatedField<Tinkoff.InvestApi.V1.Account> accounts)
    {
        foreach (var account in accounts)
        {
            yield return _mapper.Map<Account>(account);
        }
    }

    private IEnumerable<HistoricCandle> ToDomain(RepeatedField<Tinkoff.InvestApi.V1.HistoricCandle> candles)
    {
        foreach (var candle in candles)
        {
            yield return _mapper.Map<HistoricCandle>(candle);
        }
    }
}
