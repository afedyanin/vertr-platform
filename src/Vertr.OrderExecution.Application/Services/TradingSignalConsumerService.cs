using Microsoft.Extensions.DependencyInjection;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common;
using Vertr.Platform.Common.Channels;

namespace Vertr.OrderExecution.Application.Services;

internal class TradingSignalConsumerService : DataConsumerServiceBase<TradingSignal>
{
    private readonly ICommandHandler<TradingSignalCommand, ExecuteOrderResponse> _tradingSignalHandler;
    public TradingSignalConsumerService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _tradingSignalHandler = serviceProvider.GetRequiredService<ICommandHandler<TradingSignalCommand, ExecuteOrderResponse>>();
    }

    protected override async Task Handle(TradingSignal data, CancellationToken cancellationToken = default)
    {
        var command = new TradingSignalCommand
        {
            RequestId = data.RequestId,
            InstrumentId = data.InstrumentId,
            QtyLots = data.QtyLots,
            PortfolioIdentity = data.PortfolioIdentity,
        };

        _ = await _tradingSignalHandler.Handle(command, cancellationToken);
    }
}
