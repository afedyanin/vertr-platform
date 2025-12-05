namespace Vertr.CommandLine.Models;

public record class FileDataSource
{
    public required string Symbol { get; init; }

    public required string FilePath { get; init; }
}