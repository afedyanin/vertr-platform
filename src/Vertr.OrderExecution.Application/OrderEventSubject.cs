using System.Reactive.Subjects;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;

namespace Vertr.OrderExecution.Application;

internal class OrderEventSubject : IOrderEventObservable, IOrderEventHandler, IDisposable
{
    private readonly Subject<OrderEvent> _orderEventSubject;

    public IObservable<OrderEvent> StreamOrderEvents() => _orderEventSubject;

    public OrderEventSubject()
    {
        _orderEventSubject = new Subject<OrderEvent>();
    }

    public Task HandleOrderEvent(OrderEvent orderEvent)
    {
        _orderEventSubject.OnNext(orderEvent);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _orderEventSubject?.Dispose();
    }
}
