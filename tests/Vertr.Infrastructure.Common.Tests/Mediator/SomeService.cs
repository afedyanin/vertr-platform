namespace Vertr.Infrastructure.Common.Tests.Mediator;

public interface ISomeService
{
    public string Message { get; set; }
}

internal class SomeService : ISomeService
{
    public string Message { get; set; } = "";
}
