using Microsoft.Extensions.DependencyInjection;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.Services;

internal class StrategyHostingService : DataConsumerServiceBase<StrategyMetadata>, IStrategyHostingService
{
    private Dictionary<Guid, IStrategy> _strategies;

    private readonly IStrategyMetadataRepository _repository;

    public StrategyHostingService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _repository = ServiceProvider.GetRequiredService<IStrategyMetadataRepository>();
    }

    public Task<IStrategy[]> GetActiveStrategies()
        => Task.FromResult(_strategies.Values.ToArray());

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _strategies = await InitStrategies();
        await base.ExecuteAsync(stoppingToken);
    }

    protected override Task Handle(StrategyMetadata data, CancellationToken cancellationToken = default)
    {
        // TODO: Update strategies list
        return Task.CompletedTask;
    }

    private async Task<Dictionary<Guid, IStrategy>> InitStrategies()
    {
        var res = new Dictionary<Guid, IStrategy>();

        var strategiesMeta = await _repository.GetAll();

        foreach (var metadata in strategiesMeta)
        {
            if (!metadata.IsActive)
            {
                continue;
            }

            var strategy = StrategyFactory.Create(metadata, ServiceProvider);
            res[strategy.Id] = strategy;
        }

        return res;
    }
}
