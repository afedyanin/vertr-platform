using MediatR;

namespace Vertr.Application.Orders;

internal class OrderApprovementHandler : IRequestHandler<OrderApprovementRequest, OrderApprovementResponse>
{
    public Task<OrderApprovementResponse> Handle(
        OrderApprovementRequest request,
        CancellationToken cancellationToken)
    {
        // забрать последний снапшот портфеля по accountID
        // выбрать позиции из снапшота
        // найти позицию по символу
        // найти количество в позиции

        // Get portfolio position for accountId
        // convert to lots
        // if pos == 0 => return signalQty
        // if sign(pos) == sign(signalQty) => return 0 
        // if sign(pos) != sign(signalQty) => return signalQty + pos
        // check cash available?
        throw new NotImplementedException();
    }
}
