namespace Vertr.Common.Application.Abstractions;

public interface IEventHandler<T>
{
    public ValueTask OnEvent(T data);
}
