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
    private readonly ITradingGateway _tradingGateway;
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly ILogger<PortfolioSubscriber> _logger;

    private Instrument[] _instruments = [];

    // TODO: Get predictors from config or args
    private readonly string[] _predictors = ["RandomWalk"];

    protected override bool IsEnabled => false;
    protected override RedisChannel RedisChannel => new RedisChannel("portfolios", PatternMode.Literal);

    public PortfolioSubscriber(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        _tradingGateway = serviceProvider.GetRequiredService<ITradingGateway>();
        _logger = LoggerFactory.CreateLogger<PortfolioSubscriber>();
    }

    protected override async ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        await base.OnBeforeStart(cancellationToken);
        _instruments = await _tradingGateway.GetAllInstruments();
        _portfolioRepository.Init(_predictors);
    }

    public override void HandleSubscription(RedisChannel channel, RedisValue message)
    {
        var portfolio = Portfolio.FromJson(message.ToString());

        if (portfolio == null)
        {
            _logger.LogWarning($"Cannot deserialize portfolio.");
            return;
        }

        _portfolioRepository.Update(portfolio);
        var predictor = _portfolioRepository.GetPredictor(portfolio.Id);
        _logger.LogInformation(portfolio.Dump(predictor, _instruments));
    }
}