using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Application.Abstractions;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Controllers;
[Route("subscriptions")]
[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ICandleSubscriptionsRepository _repository;

    public SubscriptionsController(ICandleSubscriptionsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subscriptions = await _repository.GetAll();

        // TODO Convert
        return Ok(subscriptions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var subscription = await _repository.GetById(id);

        if (subscription == null)
        {
            return NotFound();
        }

        // TODO Convert
        return Ok(subscription);
    }

    [HttpGet("by-symbol/{symbol}")]
    public async Task<IActionResult> GetBySymbol(string symbol)
    {
        var subscriptions = await _repository.GetBySymbol(symbol);

        // TODO Convert
        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CandleSubscriptionRequest request)
    {
        var existingSubscriptions = await _repository.GetBySymbol(request.Symbol);

        var found = existingSubscriptions.FirstOrDefault(s => s.CandleInterval == request.CandleInterval);

        if (found != null)
        {
            return Ok(found);
        }

        var subscription = new CandleSubscription
        {
            Id = Guid.NewGuid(),
            Symbol = request.Symbol,
            CandleInterval = request.CandleInterval,
            WaitingClose = request.WaitingClose,
        };

        var saved = await _repository.Save(subscription);

        if (!saved)
        {
            return BadRequest($"Cannot save CandleSubscriptionRequest: {request}");
        }

        return Ok(subscription);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.Delete(id);
        return Ok(deleted);
    }
}
