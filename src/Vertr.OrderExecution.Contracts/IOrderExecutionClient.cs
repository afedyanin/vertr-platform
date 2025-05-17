using Refit;

namespace Vertr.OrderExecution.Contracts;

public interface IOrderExecutionClient
{
    [Post("/positions/open")]
    public Task<OrderExecutionResponse> OpenPosition(OpenPositionRequest request);

    [Post("/positions/close")]
    public Task<OrderExecutionResponse> ClosePosition(ClosePositionRequest request);

    [Post("/positions/revert")]
    public Task<OrderExecutionResponse> RevertPosition(RevertPositionRequest request);

    [Post("/signals")]
    public Task<OrderExecutionResponse> Porocess(TradingSignalRequest request);
}
