using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vertr.OrderExecution.Application;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Commands;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("api/portfolios")]
[ApiController]
public class PortfoliosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly OrderExecutionSettings _orderExecutionSettings;

    public PortfoliosController(
        IMediator mediator,
        IPortfolioRepository portfolioRepository,
        IOptions<OrderExecutionSettings> orderOptions)
    {
        _mediator = mediator;
        _portfolioRepository = portfolioRepository;
        _orderExecutionSettings = orderOptions.Value;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var portfolios = await _portfolioRepository.GetAll();
        return Ok(portfolios);
    }

    [HttpGet("{portfolioId:guid}")]
    public async Task<IActionResult> GetPortfolio(Guid portfolioId)
    {
        var portfolio = await _portfolioRepository.GetById(portfolioId);
        return Ok(portfolio);
    }

    [HttpPost()]
    public async Task<IActionResult> SavePortfolio(Portfolio portfolio)
    {
        var saved = await _portfolioRepository.Save(portfolio);

        if (!saved)
        {
            return BadRequest($"Cannot save porfolio Id={portfolio.Id}");
        }

        return Ok(portfolio.Id);
    }

    [HttpPut("deposit/{portfolioId:guid}")]
    public async Task<IActionResult> PayIn(Guid portfolioId, decimal amount, string currency = "rub")
    {
        var request = new DepositAmountRequest
        {
            AccountId = _orderExecutionSettings.AccountId,
            PortfolioId = portfolioId,
            Amount = new Money(amount, currency),
        };

        await _mediator.Send(request);

        return Ok();
    }

    [HttpPut("position-overrides/{portfolioId:guid}")]
    public async Task<IActionResult> OverridePositions(Guid portfolioId, PositionOverride[] overrides)
    {
        var req = new OverridePositionsRequest
        {
            AccountId = _orderExecutionSettings.AccountId,
            PortfolioId = portfolioId,
            Overrides = overrides
        };

        await _mediator.Send(req);

        return Ok();
    }
}
