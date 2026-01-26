namespace Vertr.Common.Application;

public record class OrderBookChangedEvent
{
    public long Sequence { get; set; }
}
