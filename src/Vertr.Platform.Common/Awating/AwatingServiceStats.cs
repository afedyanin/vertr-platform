
using System.Text;

namespace Vertr.Platform.Common.Awating;
public class AwatingServiceStats<T> where T : struct
{
    public T[] KeysAdded { get; init; } = [];

    public T[] KeysCompleted { get; init; } = [];

    public T[] KeysCancelled { get; init; } = [];

    public T[] KeysAwaiting { get; init; } = [];

    public override string? ToString()
    {
        var sb = new StringBuilder();

        var added = string.Join(", ", KeysAdded);
        sb.AppendLine($"Keys added=[{added}]");

        var completed = string.Join(", ", KeysCompleted);
        sb.AppendLine($"Keys completed=[{completed}]");

        var cancelled = string.Join(", ", KeysCancelled);
        sb.AppendLine($"Keys cancelled=[{cancelled}]");

        var awating = string.Join(", ", KeysAwaiting);
        sb.AppendLine($"Keys awating=[{awating}]");

        return sb.ToString();
    }
}
