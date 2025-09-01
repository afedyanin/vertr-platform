using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/order-events")]
[ApiController]
public class OrderEventsController : ControllerBase
{
    private readonly IOrderEventRepository _orderEventRepository;

    public OrderEventsController(IOrderEventRepository orderEventRepository)
    {
        _orderEventRepository = orderEventRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrderEvents(int limit = 1000)
    {
        var res = await _orderEventRepository.GetAll(limit);

        return Ok(res);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderEvent(Guid id)
    {
        var res = await _orderEventRepository.GetById(id);

        return Ok(res);
    }

    [HttpGet("portfolio/{portfolioId:guid}")]
    public async Task<IActionResult> GetOrderEventsByPortfolio(Guid portfolioId)
    {
        var res = await _orderEventRepository.GetByPortfolioId(portfolioId);

        return Ok(res);
    }

    [HttpDelete("{portfolioId:guid}")]
    public async Task<IActionResult> DeleteOrderEvents(Guid portfolioId)
    {
        var res = await _orderEventRepository.DeleteByPortfolioId(portfolioId);
        return Ok(res);
    }
}
