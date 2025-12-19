using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface ICandleProcessingPipeline
{
    public Task Start(bool verbose = true, CancellationToken cancellationToken = default);

    public void Handle(Candle candle);

    public Task Stop();
}
