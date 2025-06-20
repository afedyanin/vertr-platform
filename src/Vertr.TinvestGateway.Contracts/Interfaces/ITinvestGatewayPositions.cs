using Vertr.OrderExecution.Contracts;

namespace Vertr.TinvestGateway.Contracts.Interfaces;

public interface ITinvestGatewayPositions
{
    public Task<Operation[]?> GetOperations(
        string accountId,
        DateTime? from = null,
        DateTime? to = null);

    public Task<PositionsResponse?> GetPositions(string accountId);

    public Task<PortfolioResponse?> GetPortfolio(string accountId);
}
