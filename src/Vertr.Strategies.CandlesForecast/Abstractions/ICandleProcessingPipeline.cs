using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Strategies.CandlesForecast.Abstractions;

public interface ICandleProcessingPipeline : IEventProcessingPipeline<CandleReceivedEvent>
{
    public void Handle(Candle candle);
}
