namespace Vertr.CommandLine.Models.Requests;

public abstract class ResponseBase
{
    public string? Message { get; init; }

    public Exception? Exception { get; init; }

    public bool HasErrors => Exception != null;
}