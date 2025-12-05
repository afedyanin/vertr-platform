using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vertr.TinvestGateway.Contracts;

public static class JsonOptions
{
    private static JsonSerializerOptions? _options;

    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            if (_options == null)
            {
                _options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                _options.Converters.Add(new JsonStringEnumConverter());
            }

            return _options;
        }
    }
}