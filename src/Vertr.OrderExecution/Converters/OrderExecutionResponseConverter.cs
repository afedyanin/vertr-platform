using Vertr.OrderExecution.Application.Commands;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Converters;

internal static class OrderExecutionResponseConverter
{
    public static OrderExecutionResponse Convert(this ClosePositionResponse response)
        => new OrderExecutionResponse(
            response.PostOrderResult?.OrderId,
            response.Message);

    public static OrderExecutionResponse Convert(this OpenPositionResponse response)
        => new OrderExecutionResponse(
            response.PostOrderResult?.OrderId,
            response.Message);

    public static OrderExecutionResponse Convert(this ReversePositionResponse response)
        => new OrderExecutionResponse(
            response.PostOrderResult?.OrderId,
            response.Message);

    public static OrderExecutionResponse Convert(this TradingSignalResponse response)
        => new OrderExecutionResponse(
            response.PostOrderResult?.OrderId,
            response.Message);
}
