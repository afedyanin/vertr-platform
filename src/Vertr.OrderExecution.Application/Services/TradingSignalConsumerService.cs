using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts;
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
        var command = new TradingSignalCommand
        {
            RequestId = data.Id,
            InstrumentId = data.InstrumentId,
            QtyLots = data.QtyLots,
            // TODO: Fix it
            PortfolioIdentity = new PortfolioIdentity("data.AccountId", data.SubAccountId),
        };

        _ = await _mediator.Send(command, cancellationToken);
    }
}
