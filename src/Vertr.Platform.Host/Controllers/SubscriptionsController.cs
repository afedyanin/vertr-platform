using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/subscriptions")]
[ApiController]

public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public SubscriptionsController(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var instruments = await _subscriptionsRepository.GetAll();
        return Ok(instruments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var instrument = await _subscriptionsRepository.GetById(id);

        if (instrument == null)
        {
            return NotFound();
        }

        return Ok(instrument);
    }

    [HttpPost()]
    public async Task<IActionResult> Save([FromBody] CandleSubscription subscription)
    {
        var saved = await _subscriptionsRepository.Save(subscription);
        if (!saved)
        {
            return BadRequest($"Cannot save subscription. Id={subscription.Id}");
        }
        return Ok(subscription.Id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deletedCount = await _subscriptionsRepository.Delete(id);
        return Ok();
    }
}
