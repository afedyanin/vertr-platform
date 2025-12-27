using Refit;
using Vertr.Common.ForecastClient.Models;

namespace Vertr.Common.ForecastClient;

public interface IVertrForecastClient
{
    [Get("/stats-forecast/all-keys")]
    Task<string[]> GetKeysStats();

    [Post("/stats-forecast")]
    Task<ForecastItem[]> ForecastStats(ForecastRequest request);

    [Get("/ml-forecast/all-keys")]
    Task<string[]> GetKeysMl();

    [Post("/ml-forecast")]
    Task<ForecastItem[]> ForecastMl(ForecastRequest request);

    [Post("/neural-forecast/AutoLSTM")]
    Task<decimal> AutoLSTM(SeriesItem[] series);

    [Post("/neural-forecast/AutoRNN")]
    Task<decimal> AutoRNN(SeriesItem[] series);

    [Post("/neural-forecast/AutoMLP")]
    Task<decimal> AutoMLP(SeriesItem[] series);

    [Post("/neural-forecast/AutoDeepAR")]
    Task<decimal> AutoDeepAR(SeriesItem[] series);

    [Post("/neural-forecast/AutoDeepNPTS")]
    Task<decimal> AutoDeepNPTS(SeriesItem[] series);

    [Post("/neural-forecast/AutoKAN")]
    Task<decimal> AutoKAN(SeriesItem[] series);

    [Post("/neural-forecast/AutoTFT")]
    Task<decimal> AutoTFT(SeriesItem[] series);

    [Post("/neural-forecast/AutoTimesNet")]
    Task<decimal> AutoTimesNet(SeriesItem[] series);
}
