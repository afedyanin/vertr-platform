using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.Application.Commands;
public class TradingSignalResponse
{
    public PostOrderResult? PostOrderResult { get; init; }

    public string? Message { get; init; }
}
