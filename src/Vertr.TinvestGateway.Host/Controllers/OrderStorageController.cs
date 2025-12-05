using Microsoft.AspNetCore.Mvc;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Host.Controllers;

[Route("api/order-storage")]
[ApiController]
public class OrderStorageController : ControllerBase
{
    private readonly IOrderTradeRepository _orderTradeRepository;
    private readonly IOrderStateRepository _orderStateRepository;
    private readonly IOrderRequestRepository _orderRequestRepository;
    private readonly IOrderResponseRepository _orderResponseRepository;
    private readonly IPortfolioRepository _portfolioRepository;

    public OrderStorageController(
        IOrderTradeRepository orderTradeRepository,
        IOrderStateRepository orderStateRepository,
        IOrderRequestRepository orderRequestRepository,
        IOrderResponseRepository orderResponseRepository,
        IPortfolioRepository portfolioRepository)
    {
        _orderTradeRepository = orderTradeRepository;
        _orderStateRepository = orderStateRepository;
        _orderRequestRepository = orderRequestRepository;
        _orderResponseRepository = orderResponseRepository;
        _portfolioRepository = portfolioRepository;
    }

    [HttpGet("trades")]
    public async Task<IActionResult> FindOrderTrades([FromQuery] string pattern)
    {
        var trades = await _orderTradeRepository.Find(pattern);
        return Ok(trades);
    }

    [HttpGet("states")]
    public async Task<IActionResult> FingOrderStates([FromQuery] string pattern)
    {
        var items = await _orderStateRepository.Find(pattern);
        return Ok(items);
    }

    [HttpGet("requests")]
    public async Task<IActionResult> FindOrderRquests([FromQuery] string pattern)
    {
        var requests = await _orderRequestRepository.Find(pattern);
        return Ok(requests);
    }

    [HttpGet("requests/{requestId:guid}")]
    public async Task<IActionResult> GetOrderRquest(Guid requestId)
    {
        var request = await _orderRequestRepository.Get(requestId);

        if (request == null)
        {
            return NotFound();
        }

        return Ok(request);
    }

    [HttpGet("responses")]
    public async Task<IActionResult> FindOrderResponses([FromQuery] string pattern)
    {
        var responses = await _orderResponseRepository.Find(pattern);
        return Ok(responses);
    }



    [HttpGet("responses/{orderId:guid}")]
    public async Task<IActionResult> GetOrderResponse(Guid orderId)
    {
        var response = await _orderResponseRepository.Get(orderId);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpGet("portfolio/{portfolioId:guid}")]
    public async Task<IActionResult> GetPortfolio(Guid portfolioId)
    {
        var portfolio = await _portfolioRepository.GetById(portfolioId);

        if (portfolio == null)
        {
            return NotFound();
        }

        return Ok(portfolio);
    }
}