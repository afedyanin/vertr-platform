using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.TinvestGateway.Application;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Interfaces;
using Vertr.MarketData.Application;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.DataAccess;
using Vertr.PortfolioManager.Application;
using Vertr.PortfolioManager.DataAccess;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.OrderExecution.Contracts;
using MediatR;

namespace Vertr.Paltform.Tests;

public abstract class ApplicationTestBase
{
    private const decimal _initialAmount = 100_000;
    private const string _connStringName = "VertrDbConnection";
    private static readonly Guid _sberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    private readonly ITinvestGatewayAccounts _tinvestGatewayAccounts;
    private readonly IPortfolioManager _portfolioManager;

    private readonly IConfiguration _configuration;
    private readonly ServiceProvider _serviceProvider;
    private readonly IMediator _mediator;

    public ApplicationTestBase()
    {

        var services = new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        _configuration = configurationBuilder.Build();
        services.AddSingleton(_ => _configuration);

        var connectionString = _configuration.GetConnectionString(_connStringName);

        services.AddTinvestGateway(_configuration);
        services.AddMarketData();
        services.AddOrderExecution();
        services.AddOrderExecutionDataAccess(connectionString!);
        services.AddPortfolioManager();
        services.AddPortfolioManagerDataAccess(connectionString!);

        _serviceProvider = services.BuildServiceProvider();

        _tinvestGatewayAccounts = _serviceProvider.GetRequiredService<ITinvestGatewayAccounts>();
        _portfolioManager = _serviceProvider.GetRequiredService<IPortfolioManager>();
        _mediator = _serviceProvider.GetRequiredService<IMediator>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _serviceProvider.Dispose();
    }

    protected async Task<string> OpenAccount(decimal amount = _initialAmount)
    {
        var accountId = await _tinvestGatewayAccounts.CreateSandboxAccount("application test");
        _ = await _tinvestGatewayAccounts.PayIn(accountId, new Money(amount));
        return accountId;
    }

    protected async Task CloseAccount(string accountId)
    {
        await _tinvestGatewayAccounts.CloseSandboxAccount(accountId);
    }

    protected async Task<PortfolioSnapshot?> GetPortfolio(string accountId)
    {
        var res = await _portfolioManager.MakeSnapshot(accountId);
        return res;
    }

    protected async Task<ExecuteOrderResponse> OpenPosition(PortfolioIdentity portfolioId, long qtyLots)
    {
        var req = new OpenPositionRequest
        {
            RequestId = Guid.NewGuid(),
            PortfolioId = portfolioId,
            QtyLots = qtyLots,
            InstrumentId = _sberId,
        };

        var res = await _mediator.Send(req);

        return res;
    }

    protected async Task<ExecuteOrderResponse> ReversePosition(PortfolioIdentity portfolioId)
    {
        var req = new ReversePositionRequest
        {
            RequestId = Guid.NewGuid(),
            PortfolioId = portfolioId,
            InstrumentId = _sberId,
        };

        var res = await _mediator.Send(req);

        return res;
    }
}
