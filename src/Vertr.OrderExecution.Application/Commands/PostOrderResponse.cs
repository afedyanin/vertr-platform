using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.Application.Commands;
public class PostOrderResponse
{
    public required PostOrderResult PostOrderResult { get; init; }
}
