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
        var result = response.Convert(accountId);

        return Ok(result);
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
        var portfolio = response.Convert();

        return Ok(portfolio);
    }
}
