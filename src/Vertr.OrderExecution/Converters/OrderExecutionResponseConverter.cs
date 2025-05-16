using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Converters;

internal static class OrderExecutionResponseConverter
{
    public static OrderExecutionResponse Convert(this Application.Commands.OrderExecutionResponse response)
        => new OrderExecutionResponse(
            response.OrderId,
            response.ErrorMessage);
}
