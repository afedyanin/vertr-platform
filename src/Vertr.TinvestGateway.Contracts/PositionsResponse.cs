namespace Vertr.TinvestGateway.Contracts;
public record class PositionsResponse
{
    public Money[] Money { get; init; } = [];

    public Money[] Blocked { get; init; } = [];

    public Position[] Securities {  get; init; } = [];

    public Position[] Futures { get; init; } = [];

    public Position[] Options { get; init; } = [];
}
