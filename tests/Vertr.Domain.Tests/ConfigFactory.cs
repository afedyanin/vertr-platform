using Microsoft.Extensions.Configuration;

namespace Vertr.Domain.Tests;

internal static class ConfigFactory
{
    public static IConfiguration GetConfiguration(string fileName = "appsettings.test.json")
    {
        var config = new ConfigurationBuilder()
           .AddJsonFile(fileName)
           .AddEnvironmentVariables()
           .Build();

        return config;
    }
}
