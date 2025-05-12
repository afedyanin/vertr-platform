using System.Text.Json;
using Confluent.Kafka;

namespace Vertr.Infrastructure.Kafka;
internal sealed class DefaultJsonSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    internal readonly JsonSerializerOptions _jsonSerializerOptions;

    public DefaultJsonSerializer(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions();
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        if (data == null)
        {
            return [];
        }

        var bytes = JsonSerializer.SerializeToUtf8Bytes(data, _jsonSerializerOptions);

        return bytes;
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
#pragma warning disable CS8603 // Possible null reference return.
        if (isNull)
        {
            return default;
        }

        var res = JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions);
        return res;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
