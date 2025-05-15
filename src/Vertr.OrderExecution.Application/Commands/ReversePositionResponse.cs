using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.Application.Commands;
public class ReversePositionResponse
{
    public PostOrderResult? PostOrderResult { get; init; }

    public string? Message { get; init; }
}
