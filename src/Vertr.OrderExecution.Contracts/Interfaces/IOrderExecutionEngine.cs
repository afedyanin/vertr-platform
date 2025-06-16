namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderExecutionEngine
{
    public Task<OrderExecutionResponse> OpenPosition(OpenPositionRequest request);

    public Task<OrderExecutionResponse> ClosePosition(ClosePositionRequest request);

    public Task<OrderExecutionResponse> RevertPosition(RevertPositionRequest request);

    public Task<OrderExecutionResponse> PorocessTradingSignal(TradingSignalRequest request);
}
