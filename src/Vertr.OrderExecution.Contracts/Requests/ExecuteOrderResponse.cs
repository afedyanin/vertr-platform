namespace Vertr.OrderExecution.Contracts.Requests;
public class ExecuteOrderResponse
{
    public string? OrderId { get; init; }

    public string? ErrorMessage { get; init; }
}
