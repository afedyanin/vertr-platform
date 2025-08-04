using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vertr.Platform.Common.Utils;
public static class JsonOptions
{
    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }
}
