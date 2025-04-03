using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vertr.Terminal.Shared.Services;

namespace Vertr.Terminal.Shared.Tests;

public class QuotesSourceTests
{
    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
    };

    [Test]
    public void CanSerializeQuotesSource()
    {
        var quotes = QuotesSource.Init();

        _serializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse());

        var json = JsonSerializer.Serialize(quotes, _serializerOptions);

        Assert.That(json, Is.Not.Null);

        Console.WriteLine(json);
    }
}

public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));
        return DateTime.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // "2025-04-03T20:18:50.8996533+03:00"
        // writer.WriteStringValue(value.ToString("o"));
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
    }
}

