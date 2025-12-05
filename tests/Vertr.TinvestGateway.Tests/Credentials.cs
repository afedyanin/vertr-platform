using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.Tests;

internal static class Credentials
{
    public static readonly InvestApiSettings ApiSettings = new InvestApiSettings()
    {
        AccessToken = "<API_KEY>",
        AppName = "VERTR",
        Sandbox = true,
    };
}