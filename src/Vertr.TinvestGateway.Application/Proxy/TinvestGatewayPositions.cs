using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Vertr.OrderExecution.Contracts;
using Vertr.TinvestGateway.Application.Converters;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.TinvestGateway.Application.Proxy;

internal class TinvestGatewayPositions : TinvestGatewayBase, ITinvestGatewayPositions
{
    public TinvestGatewayPositions(InvestApiClient investApiClient) : base(investApiClient)
    {
    }

    public async Task<Operation[]?> GetOperations(string accountId, DateTime? from = null, DateTime? to = null)
    {
        var request = new Tinkoff.InvestApi.V1.OperationsRequest
        {
            AccountId = accountId,
        };

        if (from.HasValue)
        {
            request.From = Timestamp.FromDateTime(from.Value);
        }

        if (to.HasValue)
        {
            request.To = Timestamp.FromDateTime(to.Value);
        }

        var response = await InvestApiClient.Operations.GetOperationsAsync(request);
        var operations = response.Operations.ToArray().Convert();

        return operations;
    }


    public async Task<PortfolioResponse?> GetPortfolio(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PortfolioRequest
        {
            AccountId = accountId,
            Currency = Tinkoff.InvestApi.V1.PortfolioRequest.Types.CurrencyRequest.Rub
        };

        var response = await InvestApiClient.Operations.GetPortfolioAsync(request);
        var portfolio = response.Convert();

        return portfolio;
    }

    public async Task<PositionsResponse?> GetPositions(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PositionsRequest
        {
            AccountId = accountId,
        };

        var response = await InvestApiClient.Operations.GetPositionsAsync(request);
        var result = response.Convert(accountId);

        return result;
    }
}
