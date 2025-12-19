using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface ICandleProcessingPipeline
{
    public Task OnBeforeStart(CancellationToken cancellationToken = default);

    public void Handle(Candle candle);

    public Task OnBeforeStop(bool verbose = false);
}
