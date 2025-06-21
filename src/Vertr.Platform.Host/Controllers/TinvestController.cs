using Microsoft.AspNetCore.Mvc;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.Server.Controllers;
[Route("tinvest")]
[ApiController]
public class TinvestController : ControllerBase
{
    private readonly ITinvestGatewayAccounts _tinvestGatewayAccounts;
    private readonly ITinvestGatewayMarketData _tinvestGatewayMarketData;
    private readonly ITinvestGatewayOrders _tinvestGatewayOrders;

    public TinvestController(
        ITinvestGatewayAccounts tinvestGatewayAccounts,
        ITinvestGatewayMarketData tinvestGatewayMarketData,
        ITinvestGatewayOrders tinvestGatewayOrders
        )
    {
        _tinvestGatewayAccounts = tinvestGatewayAccounts;
        _tinvestGatewayMarketData = tinvestGatewayMarketData;
        _tinvestGatewayOrders = tinvestGatewayOrders;
    }

    [HttpGet("sandbox-accounts")]
    public async Task<IActionResult> GetSandboxAccount()
    {
        var accounts = await _tinvestGatewayAccounts.GetSandboxAccounts();
        return Ok(accounts);
    }
}
