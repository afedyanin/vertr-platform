using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.Tests;
internal static class Credentials
{
    public static readonly InvestApiSettings ApiSettings = new InvestApiSettings()
    {
        AccessToken = "t.8DpIsag8_t2bHcaPEXZiAxDLdxbyqP7MXvDwoamPBWSDBD7dgQeMNutgas5Ay83YOlLsA-m8qSPm8Sz-FMaNuw",
        AppName = "VERTR",
        Sandbox = true,
    };
}
