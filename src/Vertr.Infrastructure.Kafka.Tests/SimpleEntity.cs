namespace Vertr.Infrastructure.Kafka.Tests;

internal sealed record class SimpleEntity(Guid Id, string Name, decimal Price, DateTime Time, Status Status)
{
    public static SimpleEntity Create(int index)
    {
        return new SimpleEntity(
            Guid.NewGuid(),
            $"Name_{index}",
            1234.56m + index,
            DateTime.Now,
            (Status)(index % 3)
            );
    }
}

internal enum Status
{
    Undefined = 0,
    Success = 1,
    Error = 2,
}
