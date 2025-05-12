using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Requests;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Controllers;
[Route("api/accounts")]
[ApiController]
public class AccountsController : TinvestControllerBase
{

    public AccountsController(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient) : base(options, investApiClient)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var response = await InvestApiClient.Users.GetAccountsAsync();
        var accounts = response.Accounts.ToArray().ToAccounts();

        return Ok(accounts);
    }

    [HttpGet("sandbox")]
    public async Task<IActionResult> GetSandboxAccounts()
    {
        var response = await InvestApiClient.Sandbox.GetSandboxAccountsAsync(new GetAccountsRequest());
        var accounts = response.Accounts.ToArray().ToAccounts();

        return Ok(accounts);
    }

    [HttpPost("sandbox")]
    public async Task<IActionResult> CreateAccount(CreateSandboxAccountRequest request)
    {
        var tRequest = new OpenSandboxAccountRequest
        {
            Name = request.AccountName,
        };

        var response = await InvestApiClient.Sandbox.OpenSandboxAccountAsync(tRequest);

        return Ok(response.AccountId);
    }

    [HttpDelete("sandbox/{accountId}")]
    public async Task<IActionResult> CloseAccount(string accountId)
    {
        var tRequest = new CloseSandboxAccountRequest
        {
            AccountId = accountId,
        };

        _ = await InvestApiClient.Sandbox.CloseSandboxAccountAsync(tRequest);

        return Ok();
    }

    [HttpPut("sandbox/{accountId}")]
    public async Task<IActionResult> PayIn(string accountId, Money money)
    {
        var tRequest = new SandboxPayInRequest
        {
            AccountId = accountId,
            Amount = money.ToMoneyValue()
        };

        var response = await InvestApiClient.Sandbox.SandboxPayInAsync(tRequest);
        var balance = response.Balance.FromMoneyValue();

        return Ok(balance);
    }
}
