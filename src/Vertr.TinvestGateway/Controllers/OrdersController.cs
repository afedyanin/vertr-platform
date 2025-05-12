using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController : TinvestControllerBase
{
    public OrdersController(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient) : base(options, investApiClient)
    {
    }

    [HttpPost]
    public async Task<IActionResult> PostOrder(PostOrderRequest request)
    {
        var tRequest = request.Convert(Settings);
        var response = await InvestApiClient.Orders.PostOrderAsync(tRequest);
        var orderResponse = response.Convert();

        return Ok(orderResponse);
    }

    [HttpPut("cancel")]
    public async Task<IActionResult> CancelOrder(
        string accountId,
        string orderId)
    {
        var cancelOrderRequest = new Tinkoff.InvestApi.V1.CancelOrderRequest
        {
            AccountId = accountId,
            OrderId = orderId,
        };

        var response = await InvestApiClient.Orders.CancelOrderAsync(cancelOrderRequest);

        return Ok(response.Time.ToDateTime());
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetOrderState(
        string accountId,
        string orderId,
        PriceType priceType = PriceType.Unspecified)
    {
        var orderStateRequest = new Tinkoff.InvestApi.V1.GetOrderStateRequest
        {
            AccountId = accountId,
            OrderId = orderId,
            PriceType = priceType.Convert(),
        };

        var response = await InvestApiClient.Orders.GetOrderStateAsync(orderStateRequest);
        var state = response.Convert();

        return Ok(state);
    }
}
