using System.Text.Json;

namespace Vertr.Common.Contracts;


public record class InterestRate(string Ticker, DateTime TimeUtc, decimal Value)
{
    public string ToJson()
        => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static InterestRate? FromJson(string jsonString)
        => JsonSerializer.Deserialize<InterestRate>(jsonString, JsonOptions.DefaultOptions);
}
