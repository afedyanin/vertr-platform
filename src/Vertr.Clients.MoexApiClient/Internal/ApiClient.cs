using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Vertr.Clients.MoexApiClient.Extensions;
using Vertr.Clients.MoexApiClient.Models;

namespace Vertr.Clients.MoexApiClient.Internal;

internal sealed class ApiClient : IMoexApiClient
{
    private const string DateOnlyTemplate = "yyyy-MM-dd";
    private const string BaseAddress = "https://iss.moex.com/iss/";

    private readonly HttpClient _httpClient;
    private readonly CsvConfiguration _csvConfiguration;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(BaseAddress);

        _csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
        };
    }

    public async Task<IEnumerable<FutureInfo>> GetFutureInfo(params string[] tickers)
    {
        var res = new List<FutureInfo>();
        foreach (var ticker in tickers)
        {
            var items = await GetSecurityInfo(ticker);

            var futureInfo = items.ToFutureInfo();
            if (futureInfo != null)
            {
                res.Add(futureInfo);
            }
        }

        return res;
    }

    public async Task<IEnumerable<IndexRate>> GetIndexRates(string ticker, DateOnly? from = null, DateOnly? to = null)
    {
        var items = await GetIndexCandles(ticker, from, to);
        var rates = items.ToIndexRates(ticker);

        return rates;
    }

    public async Task<IEnumerable<SecurityInfoItem>> GetSecurityInfo(string ticker)
    {
        var url = $"securities/{ticker}.csv?iss.meta=on&iss.only=description";
        var items = new List<SecurityInfoItem>();

        var csvStream = await _httpClient.GetStreamAsync(url);
        using var reader = new StreamReader(csvStream, Encoding.GetEncoding(1251));
        using var csvReader = new CsvReader(reader, _csvConfiguration);

        // skip first 2 rows
        await csvReader.ReadAsync();
        await csvReader.ReadAsync();

        csvReader.ReadHeader();

        while (await csvReader.ReadAsync())
        {
            items.Add(csvReader.GetRecord<SecurityInfoItem>());
        }

        return items;
    }

    public async Task<IEnumerable<CandleItem>> GetIndexCandles(string ticker, DateOnly? from = null, DateOnly? to = null)
    {
        var dateTo = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var dateFrom = from ?? dateTo.AddDays(-30);
        var dateFromString = dateFrom.ToString(DateOnlyTemplate);
        var dateToString = dateTo.ToString(DateOnlyTemplate);

        var url = $"engines/stock/markets/index/securities/{ticker}/candles.csv?from={dateFromString}&till={dateToString}&interval=24";
        var items = new List<CandleItem>();

        var csvStream = await _httpClient.GetStreamAsync(url);
        using var reader = new StreamReader(csvStream, Encoding.GetEncoding(1251));
        using var csvReader = new CsvReader(reader, _csvConfiguration);

        // skip first 2 rows
        await csvReader.ReadAsync();
        await csvReader.ReadAsync();

        csvReader.ReadHeader();

        while (await csvReader.ReadAsync())
        {
            items.Add(csvReader.GetRecord<CandleItem>());
        }

        return items;
    }
}
