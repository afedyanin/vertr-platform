namespace Vertr.Adapters.Tinvest;
public class TinvestConfiguration
{
    // TODO: Move to appsettings.json
    private const string _token = "t.8DpIsag8_t2bHcaPEXZiAxDLdxbyqP7MXvDwoamPBWSDBD7dgQeMNutgas5Ay83YOlLsA-m8qSPm8Sz-FMaNuw";

    public const string ConfigKey = "Tinvest";

    public string Token { get; set; } = _token;

    public string AccountId { get; set; } = string.Empty;
}
