using Microsoft.Extensions.Configuration;

namespace Vertr.Adapters.DataAccess.Tests;
internal static class ConfigFactory
{
    public static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.test.json")
           .AddEnvironmentVariables()
           .Build();

        return config;
    }
}
