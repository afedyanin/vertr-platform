using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.Application.Commands;
public class OpenPositionResponse
{
    public PostOrderResult? PostOrderResult { get; init; }

    public string? Message { get; init; }
}
