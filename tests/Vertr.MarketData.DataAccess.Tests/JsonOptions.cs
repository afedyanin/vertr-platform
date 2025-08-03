using System.Text.Json;

namespace Vertr.MarketData.DataAccess.Tests;
internal static class JsonOptions
{
    public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
