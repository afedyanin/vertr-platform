using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Common.Channels;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Application.CommandHandlers;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.Services;

internal class TradeOperationConsumerService : DataConsumerServiceBase<TradeOperation>
{
    private readonly ITradeOperationRepository _tradeOperationRepository;
    private readonly IMediator _mediator;

    public TradeOperationConsumerService(
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _tradeOperationRepository = serviceProvider.GetRequiredService<ITradeOperationRepository>();
        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    protected override async Task Handle(TradeOperation data, CancellationToken cancellationToken = default)
    {
        if (data == null || !data.IsNew)
        {
            return;
        }

        var saved = await _tradeOperationRepository.Save(data);

        if (!saved)
        {
            var message = $"Cannot save trade operation. OrderId={data.OrderId}";
            throw new InvalidOperationException(message);
        }

        var request = new ApplyOperationRequest
        {
            Operation = data
        };

        await _mediator.Send(request, cancellationToken);
    }
}
