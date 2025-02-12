using MediatR;
using Microsoft.Extensions.Logging;

namespace Vertr.Application.Candles;
internal class LoadTinvestCandlesHandler : IRequestHandler<LoadTinvestCandlesRequest>
{
    private readonly ILogger<LoadTinvestCandlesHandler> _logger;

    public LoadTinvestCandlesHandler(
        ILogger<LoadTinvestCandlesHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(LoadTinvestCandlesRequest request, CancellationToken cancellationToken)
    {
        var symbols = string.Join(",", request.Symbols);

        _logger.LogInformation($"Processing LoadTinvestCandlesRequest symbols={symbols} interval={request.Interval}");
        await Task.Delay(100);
    }
}
