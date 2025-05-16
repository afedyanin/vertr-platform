using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.DataAccess.Repositories;
internal class OrderEventRepository : IOrderEventRepository
{
    public Task<bool> Save(OrderEvent[] events)
    {
        throw new NotImplementedException();
    }
}
