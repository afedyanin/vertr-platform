using System.Text;

namespace Vertr.Strategies.Contracts.Interfaces;
public class PredictionResult
{
    private readonly Dictionary<string, object> _dict;

    public PredictionResult(Dictionary<string, object>? dict)
    {
        _dict = dict ?? [];
    }

    public object? GetValue(string key)
    {
        _dict.TryGetValue(key, out var value);
        return value;
    }

    public T? GetValue<T>(string key) where T : class
    {
        _dict.TryGetValue(key, out var value);
        return value as T;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var kvp in _dict)
        {
            sb.AppendLine($"{kvp.Key}={kvp.Value}");
        }

        return sb.ToString();
    }
}
