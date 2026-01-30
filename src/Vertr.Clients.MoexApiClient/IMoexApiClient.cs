using Refit;

namespace Vertr.Clients.MoexApiClient;

/// <summary>
/// https://iss.moex.com/iss/reference/
/// </summary>
public interface IMoexApiClient
{
    [Get("/securities.json")]
    Task<string> GetAllSecurities();

    [Get("/index.json")]
    Task<string> GetMetadata();

    [Get("/engines/stock/markets/index/securities/RUSFAR.json")]
    Task<string> GetRusfar();

    [Get("/statistics/engines/stock/markets/index/rusfar.json")]
    Task<string> GetRusfarDetails();

    [Get("/engines/stock/markets/index/securities/RUSFAR3M/candles.json?from=2025-12-01&till=2026-01-21&interval=24")]
    Task<string> GetRusfarCandles();
}
