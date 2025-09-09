namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderEventObservable
{
    public IObservable<OrderEvent> StreamOrderEvents();
}
