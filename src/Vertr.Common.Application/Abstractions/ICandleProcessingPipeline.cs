using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface ICandleProcessingPipeline
{
    public void Handle(Candle candle);

    public ValueTask OnBeforeStart(CancellationToken cancellationToken = default);

    public ValueTask OnBeforeStop();
}
