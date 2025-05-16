namespace Vertr.OrderExecution.Contracts;
public record class OrderExecutionResponse
    (
    string? OrderId,
    string? ErrorMessage
    );
