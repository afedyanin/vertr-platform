using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Contracts;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.Strategies.Hosting.BackgroundServices;

public class PortfolioSubscriber : RedisServiceBase
{
    private readonly IInstrumentsLocalStorage _instrumentsRepository;
    private readonly IPortfoliosLocalStorage _portfolioRepository;
    private readonly ILogger<PortfolioSubscriber> _logger;

    private Instrument[] _instruments = [];
    private readonly string[] _portfolios;

    protected override RedisChannel RedisChannel => new RedisChannel(Subscriptions.Portfolios.Channel, PatternMode.Literal);
    protected override bool IsEnabled => Subscriptions.Portfolios.IsEnabled;

    public PortfolioSubscriber(
        IServiceProvider serviceProvider,
        IConfiguration configuration) : base(serviceProvider, configuration)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfoliosLocalStorage>();
        _instrumentsRepository = serviceProvider.GetRequiredService<IInstrumentsLocalStorage>();

        _portfolios = configuration.GetSection("Portfolios").Get<string[]>() ?? [];
        Debug.Assert(_portfolios.Length > 0);

        _logger = LoggerFactory.CreateLogger<PortfolioSubscriber>();
    }

    protected override async ValueTask OnBeforeStart(CancellationToken cancellationToken)
    {
        await base.OnBeforeStart(cancellationToken);
        _instruments = _instrumentsRepository.GetAll();
        _portfolioRepository.Init(_portfolios);
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
        var portfolioName = _portfolioRepository.GetNameById(portfolio.Id);

        if (_instruments.Length == 0)
        {
            _instruments = _instrumentsRepository.GetAll();
        }

        _logger.LogInformation(portfolio.Dump(portfolioName, _instruments));
    }
}