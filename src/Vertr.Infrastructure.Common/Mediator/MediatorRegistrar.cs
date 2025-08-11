using Microsoft.Extensions.DependencyInjection;
using Vertr.Platform.Common.Mediator;

namespace Vertr.Infrastructure.Common.Mediator;

public static class MediatorRegistrar
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddSingleton<IMediator, Mediatr>();
        return services;
    }
}
