using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Commands;
using Vertr.Platform.Common.Mediator;

namespace Vertr.Platform.Host.Controllers;

[Route("portfolio")]
[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IPortfolioGateway _portfolioGateway;
    private readonly ILogger<PortfolioController> _logger;

    public PortfolioController(
        IMediator mediator,
        IPortfolioGateway portfolioGateway,
        IPortfolioRepository portfolioRepository,
        ILogger<PortfolioController> logger)
    {
        _mediator = mediator;
        _portfolioGateway = portfolioGateway;
        _portfolioRepository = portfolioRepository;
        _logger = logger;
    }

    [HttpGet("{portfolioId:guid}")]
    public Task<IActionResult> GetPortfolio(Guid portfolioId)
    {
        var portfolio = _portfolioRepository.GetById(portfolioId);
        return Task.FromResult<IActionResult>(Ok(portfolio));
    }

    [HttpGet("sandbox-accounts")]
    public async Task<IActionResult> GetSandboxAccounts()
    {
        var accounts = await _portfolioGateway.GetSandboxAccounts();
        return Ok(accounts);
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _portfolioGateway.GetAccounts();
        return Ok(accounts);
    }

    [HttpPost("sandbox-account")]
    public async Task<IActionResult> CreateAccount(string accountName)
    {
        var accountId = await _portfolioGateway.CreateSandboxAccount(accountName);
        return Ok(accountId);
    }

    [HttpPut("sandbox-account/{accountId}")]
    public async Task<IActionResult> PayIn(string accountId, Guid portfolioId, decimal amount, string currency = "RUB")
    {
        var money = new Money(amount, currency);
        var balance = await _portfolioGateway.PayIn(accountId, money);

        if (balance == null || balance.Value == decimal.Zero)
        {
            return BadRequest("Balance is empty");
        }

        var request = new PayInRequest
        {
            AccountId = accountId,
            PortfolioId = portfolioId,
            Amount = money,
        };

        await _mediator.Send(request);

        return Ok(balance);
    }

    [HttpDelete("sandbox-account/{accountId}")]
    public async Task<IActionResult> CloseAccount(string accountId)
    {
        await _portfolioGateway.CloseSandboxAccount(accountId);
        return Ok();
    }

    [HttpGet("gateway-portfolio/{accountId}")]
    public async Task<IActionResult> GetGatewayPortfolio(string accountId)
    {
        var portfolio = await _portfolioGateway.GetPortfolio(accountId);
        return Ok(portfolio);
    }

    [HttpGet("gateway-operations/{accountId}")]
    public async Task<IActionResult> GetGatewayOperations(string accountId, DateTime from, DateTime to)
    {
        var operations = await _portfolioGateway.GetOperations(accountId, from, to);
        _logger.LogInformation($"Total operations count={operations?.Count()}");
        return Ok(operations);
    }

    /*
    [HttpPut("gateway-operations/replay/{accountId}")]
    public async Task<IActionResult> GetGatewayOperationsReplay(string accountId, DateTime from, DateTime to)
    {
        var portfolioRepo = CreateEmptyRepository(accountId);
        var service = new TradeOperationConsumerService(portfolioRepo, _staticMarketDataProvider);

        var operations = await _portfolioGateway.GetOperations(accountId, from, to);

        if (operations != null)
        {
            foreach (var operation in operations)
            {
                var portfolio = await service.ApplyOperation(operation);
                portfolioRepo.Save(portfolio);
            }
        }

        var portfolios = portfolioRepo.GetAllPortfolios();

        return Ok(portfolios);
    }
    */

    [HttpPut("position-overrides/{accountId}/{portfolioId}")]
    public async Task<IActionResult> OverridePositions(string accountId, Guid portfolioId, PositionOverride[] overrides)
    {
        var req = new OverridePositionsRequest
        {
            AccountId = accountId,
            PortfolioId = portfolioId,
            Overrides = overrides
        };

        await _mediator.Send(req);

        return Ok();
    }
}
