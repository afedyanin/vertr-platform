using Microsoft.Extensions.DependencyInjection;

namespace Vertr.Platform.Common.Sample;

internal static class UserCommandsRegistrar
{
    public static IServiceCollection AddUserCommands(this IServiceCollection services)
    {
        // https://youtu.be/j1OUToRyVHc?t=645
        services.AddScoped<ICommandHandler<RegisterUserCommand, Guid>, RegisterUserCommandHandler>();

        return services;
    }
}
