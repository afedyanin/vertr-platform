using Refit;
using Vertr.Common.ForecastClient.Models;

namespace Vertr.Common.ForecastClient;

public interface IVertrForecastClient
{
    [Get("/stats-forecast/all-keys")]
    Task<string[]> GetKeysStats();

    [Post("/stats-forecast")]
    Task<ForecastItem[]> ForecastStats(ForecastRequest request);
}
