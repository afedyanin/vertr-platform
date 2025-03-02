using Vertr.Domain.Enums;
using Vertr.Domain.Settings;

namespace Vertr.Domain.Ports;

public interface IPredictionService
{
    Task<IEnumerable<(DateTime, TradeAction)>> Predict(
        StrategySettings strategySettings,
        int candlesCount = 200,
        bool completedOnly = false,
        string candlesSource = "tinvest");
}
