using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;

namespace Vertr.MarketData.WebApi;

[Route("api/subscriptions")]
[ApiController]

public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IDataProducer<CandleSubscription> _subscriptionsProducer;

    public SubscriptionsController(
        ISubscriptionsRepository subscriptionsRepository,
        IDataProducer<CandleSubscription> subscriptionsProducer)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _subscriptionsProducer = subscriptionsProducer;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var subscriptions = await _subscriptionsRepository.GetAll();
        return Ok(subscriptions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var subscription = await _subscriptionsRepository.GetById(id);

        if (subscription == null)
        {
            return NotFound();
        }

        return Ok(subscription);
    }

    [HttpPost()]
    public async Task<IActionResult> Save([FromBody] CandleSubscription subscription)
    {
        var saved = await _subscriptionsRepository.Save(subscription);

        if (!saved)
        {
            return BadRequest($"Cannot save subscription. Id={subscription.Id}");
        }

        // to restart market data feed
        await _subscriptionsProducer.Produce(subscription);

        return Ok(subscription.Id);
    }

    [HttpPut("restart/{id:guid}")]
    public async Task<IActionResult> Restart(Guid id)
    {
        var subscription = await _subscriptionsRepository.GetById(id);

        if (subscription == null)
        {
            return NotFound();
        }

        await _subscriptionsProducer.Produce(subscription);

        return Ok(subscription);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var subscription = await _subscriptionsRepository.GetById(id);

        if (subscription == null)
        {
            return Ok(0);
        }

        var deletedCount = await _subscriptionsRepository.Delete(id);

        // to restart market data feed
        await _subscriptionsProducer.Produce(subscription);

        return Ok(deletedCount);
    }
}
