using Vertr.Domain.Enums;

namespace Vertr.Application.Orders;
public class OrderApprovementResponse
{
    public long ApprovedQuantityLots { get; init; }

    public OrderDirection OrderDirection { get; init; }
}
