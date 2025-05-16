namespace Vertr.OrderExecution.Application.Entities;
public class PostOrderResult
{
    public string OrderId { get; init; } = string.Empty;

    public object? Request { get; init; }

    public object? Response { get; init; }
}
