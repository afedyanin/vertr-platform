namespace Vertr.Common.Application.Abstractions;

public interface IEventHandler<T> where T : IMarketDataEvent
{
    public int HandlingOrder { get; }

    public ValueTask OnEvent(T data);
}
