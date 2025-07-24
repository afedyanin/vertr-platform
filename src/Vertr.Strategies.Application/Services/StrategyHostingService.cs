using Microsoft.Extensions.DependencyInjection;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.Services;

internal class StrategyHostingService : DataConsumerServiceBase<StrategyMetadata>, IStrategyHostingService
{
    private List<IStrategy> _strategies;

    private readonly IStrategyMetadataRepository _repository;

    public StrategyHostingService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _repository = ServiceProvider.GetRequiredService<IStrategyMetadataRepository>();
    }

    public Task<IStrategy[]> GetActiveStrategies()
        => Task.FromResult(_strategies.ToArray());

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _strategies = await InitStrategies();
        await base.ExecuteAsync(stoppingToken);
    }

    protected override Task Handle(StrategyMetadata data, CancellationToken cancellationToken = default)
    {
        // TODO: Update strategies list
        throw new NotImplementedException();
    }

    private async Task<List<IStrategy>> InitStrategies()
    {
        var res = new List<IStrategy>();

        var strategiesMeta = await _repository.GetAll();

        foreach (var metadata in strategiesMeta)
        {
            if (!metadata.IsActive)
            {
                continue;
            }

            var strategy = StrategyFactory.Create(metadata, ServiceProvider);
            res.Add(strategy);
        }

        return res;
    }
}
