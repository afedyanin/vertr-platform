using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TradingConsole.BackgroundServices;

internal sealed class PortfolioSubscriber : RedisServiceBase
{
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly IPortfolioManager _portfolioManager;
    private readonly ITradingGateway _tradingGateway;

    protected override bool IsEnabled => true;
    protected override RedisChannel RedisChannel => new RedisChannel("portfolios", PatternMode.Literal);

    public PortfolioSubscriber(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        _portfolioManager = serviceProvider.GetRequiredService<IPortfolioManager>();
        _tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
    }

    protected override ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        // TODO: Get predictors from config or args
        _portfolioRepository.Init(["RandomWalk"]);
        return base.OnBeforeStart(cancellationToken);
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        Logger.LogInformation("Portfolio received: {Message}", message);

        var portfolio = Portfolio.FromJson(message.ToString());

        if (portfolio == null)
        {
            Logger.LogWarning($"Cannot deserialize portfolio.");
            return;
        }

        _portfolioRepository.Update(portfolio);
    }

    protected override async ValueTask OnBeforeStop()
    {
        Logger.LogWarning("Closing portfolios...");
        await _portfolioManager.CloseAllPositions();

        var dump = await DumpPortfolios();
        Logger.LogWarning(dump);

        await base.OnBeforeStop();
    }

    private async Task<string> DumpPortfolios()
    {
        var instruments = await _tradingGateway.GetAllInstruments();
        var portfolios = _portfolioRepository.GetAll();

        var sb = new StringBuilder();

        foreach (var kvp in portfolios)
        {
            sb.AppendLine(kvp.Value.Dump(kvp.Key, instruments, verbose: false));
        }

        return sb.ToString();
    }
}