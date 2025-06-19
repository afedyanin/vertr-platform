using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Requests;

namespace Vertr.MarketData.Application.RequestHandlers;

internal class InstrumentSnapshotReceivedHandler : IRequestHandler<InstrumentSnapshotReceived>
{
    private readonly ILogger<InstrumentSnapshotReceivedHandler> _logger;

    public InstrumentSnapshotReceivedHandler(ILogger<InstrumentSnapshotReceivedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(InstrumentSnapshotReceived request, CancellationToken cancellationToken)
    {
        // TODO: Save instruments to Redis

        var instruments = request.Instruments == null ? [] :
            request.Instruments.Select(t => $"{t.ClassCode}.{t.Ticker}").ToArray();

        var allInstruments = string.Join(',', instruments);

        _logger.LogInformation($"Instruments Snapshot received: {allInstruments}");

        return Task.CompletedTask;
    }
}
