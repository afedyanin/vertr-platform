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

        return response.Accounts;
    }
}
