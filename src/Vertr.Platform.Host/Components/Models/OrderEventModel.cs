using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class OrderEventModel
{
    public required OrderEvent OrderEvent { get; init; }

    public required Instrument Instrument { get; set; }

}
