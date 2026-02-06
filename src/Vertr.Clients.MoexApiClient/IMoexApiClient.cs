using Vertr.Clients.MoexApiClient.Models;

namespace Vertr.Clients.MoexApiClient;

public interface IMoexApiClient
{
    public Task<IEnumerable<FutureInfo>> GetFutureInfo(params string[] tickers);

    public Task<IEnumerable<IndexRate>> GetIndexRates(string ticker, DateOnly? from = null, DateOnly? to = null);

    internal Task<IEnumerable<SecurityInfoItem>> GetSecurityInfo(string ticker);

    internal Task<IEnumerable<CandleItem>> GetIndexCandles(string ticker, DateOnly? from = null, DateOnly? to = null);
}
