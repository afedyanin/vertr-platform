using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.Application.Abstractions;

public interface IOrderEventRepository
{
    public Task<bool> Save(PostOrderResult orderResult);
}
