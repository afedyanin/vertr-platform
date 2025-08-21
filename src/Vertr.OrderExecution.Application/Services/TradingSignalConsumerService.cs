using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.Strategies.Contracts;

namespace Vertr.OrderExecution.Application.Services;

internal class TradingSignalConsumerService : DataConsumerServiceBase<TradingSignal>
{
    private readonly IMediator _mediator;

    public TradingSignalConsumerService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    protected override async Task Handle(TradingSignal data, CancellationToken cancellationToken = default)
    {
        var command = new TradingSignalRequest
        {
            RequestId = data.Id,
            InstrumentId = data.InstrumentId,
            QtyLots = data.QtyLots,
            PortfolioId = data.PortfolioId,
            BacktestId = data.BacktestId,
            CreatedAt = data.CreatedAt,
            Price = data.Price,
        };

        _ = await _mediator.Send(command, cancellationToken);
    }
}
