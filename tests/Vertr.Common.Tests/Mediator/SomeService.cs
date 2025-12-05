namespace Vertr.Common.Tests.Mediator;

internal sealed class SomeService : ISomeService
{
    public string Message { get; set; } = "";
}

public interface ISomeService
{
    public string Message { get; set; }
}
