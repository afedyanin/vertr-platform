using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.Abstractions;

public interface IFuturesProcessingPipeline : IEventProcessingPipeline<OrderBookChangedEvent>
{
}
