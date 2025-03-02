using MediatR;
using Vertr.Domain;
namespace Vertr.Application.Orders;
public class ExecuteOrderRequest : IRequest<ExecuteOrderReponse>
{
    public Guid? TradingSignalId { get; init; }

    public required PostOrderRequest PostOrderRequest { get; init; }
}
