namespace Vertr.OrderExecution.Contracts.Commands;

public class ExecuteOrderResponse
{
    public string? OrderId { get; init; }

    public string? ErrorMessage { get; init; }
}
