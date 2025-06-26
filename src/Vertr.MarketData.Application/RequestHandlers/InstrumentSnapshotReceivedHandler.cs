using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.Contracts.Requests;

namespace Vertr.MarketData.Application.RequestHandlers;

internal class InstrumentSnapshotReceivedHandler : IRequestHandler<InstrumentSnapshotReceived>
{
    private readonly ILogger<InstrumentSnapshotReceivedHandler> _logger;
    private readonly IMarketInstrumentRepository _marketInstrumentRepository;

    public InstrumentSnapshotReceivedHandler(
        IMarketInstrumentRepository marketInstrumentRepository,
        ILogger<InstrumentSnapshotReceivedHandler> logger)
    {
        _logger = logger;
        _marketInstrumentRepository = marketInstrumentRepository;
    }

    public async Task Handle(InstrumentSnapshotReceived request, CancellationToken cancellationToken)
    {
        var instruments = request.Instruments ?? [];
        await _marketInstrumentRepository.Save(instruments);
    }
}
