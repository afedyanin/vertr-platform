using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Converters;

public static class OpenInterestConverter
{
    public static OpenInterest Convert(this Tinkoff.InvestApi.V1.OpenInterest oi)
        => new OpenInterest
        {
            Time = oi.Time.ToDateTime(),
            InstrumentId = Guid.Parse(oi.InstrumentUid),
            Quantity = oi.OpenInterest_,
        };
}
