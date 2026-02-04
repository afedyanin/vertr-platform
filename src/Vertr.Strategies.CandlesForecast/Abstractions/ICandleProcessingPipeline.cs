using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.CandlesForecast.Abstractions;

public interface ICandleProcessingPipeline : IEventProcessingPipeline<CandleReceivedEvent>
{
}
