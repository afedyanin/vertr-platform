using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface ICandleProcessingPipeline
{
    public Task Start(CancellationToken cancellationToken = default);

    public void Handle(Candle candle);

    public Task Stop();

    public Func<CandleReceivedEvent, ValueTask>? OnCandleEvent { get; set; }
}
