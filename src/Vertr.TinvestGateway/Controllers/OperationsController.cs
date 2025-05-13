using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Controllers;

[Route("operations")]
[ApiController]
public class OperationsController : TinvestControllerBase
{
    public OperationsController(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient) : base(options, investApiClient)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        string accountId,
        DateTime? from = null,
        DateTime? to = null)
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

        return Ok(operations);
    }

    [HttpGet("positions")]
    public async Task<IActionResult> GetPositions(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PositionsRequest
        {
            AccountId = accountId,
        };

        var response = await InvestApiClient.Operations.GetPositionsAsync(request);

        var result = response.Convert();

        return result;
    }

    [HttpGet("portfolio")]
    public async Task<IActionResult> GetPortfolio(string accountId)
    {
        var request = new Tinkoff.InvestApi.V1.PortfolioRequest
        {
            AccountId = accountId,
            Currency = Tinkoff.InvestApi.V1.PortfolioRequest.Types.CurrencyRequest.Rub
        };

        var response = await InvestApiClient.Operations.GetPortfolioAsync(request);

        var snapshot = new PortfolioSnapshot
        {
            Id = Guid.NewGuid(),
            TimeUtc = DateTime.UtcNow,
            AccountId = accountId,
            TotalAmountBonds = response.TotalAmountBonds,
            TotalAmountCurrencies = response.TotalAmountCurrencies,
            TotalAmountEtf = response.TotalAmountEtf,
            TotalAmountFutures = response.TotalAmountFutures,
            TotalAmountOptions = response.TotalAmountOptions,
            TotalAmountShares = response.TotalAmountShares,
            TotalAmountSp = response.TotalAmountSp,
            TotalAmountPortfolio = response.TotalAmountPortfolio,
            ExpectedYield = response.ExpectedYield,
        };

        var positions = new List<PortfolioPosition>();

        foreach (var item in response.Positions)
        {
            var position = new PortfolioPosition
            {
                Id = Guid.NewGuid(),
                PortfolioSnapshot = snapshot,
                PortfolioSnapshotId = snapshot.Id,
                AveragePositionPrice = item.AveragePositionPrice ?? 0m,
                AveragePositionPriceFifo = item.AveragePositionPriceFifo ?? 0m,
                Blocked = item.Blocked,
                BlockedLots = item.BlockedLots ?? 0m,
                CurrentNkd = item.CurrentNkd ?? 0m,
                CurrentPrice = item.CurrentPrice ?? 0m,
                ExpectedYield = item.ExpectedYield ?? 0m,
                ExpectedYieldFifo = item.ExpectedYieldFifo ?? 0m,
                InstrumentType = item.InstrumentType,
                InstrumentUid = new Guid(item.InstrumentUid),
                PositionUid = new Guid(item.PositionUid),
                Quantity = item.Quantity ?? 0m,
                VarMargin = item.VarMargin ?? 0m,
            };
            positions.Add(position);
        }

        snapshot.Positions = positions;

        return snapshot;
    }
}
