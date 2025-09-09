namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderEventHandler
{
    public Task HandleOrderEvent(OrderEvent orderEvent);
}
