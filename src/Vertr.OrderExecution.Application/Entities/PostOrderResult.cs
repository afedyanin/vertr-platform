namespace Vertr.OrderExecution.Application.Entities;
public class PostOrderResult
{
    public string? OrderId { get; init; } = string.Empty;

    public OrderEvent? Request { get; init; }

    public OrderEvent? Response { get; init; }
}
