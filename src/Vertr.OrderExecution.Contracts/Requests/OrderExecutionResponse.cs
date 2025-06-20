namespace Vertr.OrderExecution.Contracts.Requests;
public class OrderExecutionResponse
{
    public string? OrderId { get; init; }

    public string? ErrorMessage { get; init; }
}
