using AutoMapper;
using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;

internal static class PositionResponseConverter
{
    public static IEnumerable<PositionSnapshot> Convert(
        this Tinkoff.InvestApi.V1.PositionsResponse response,
        string accountId,
        IMapper mapper)
    {
        var result = new List<PositionSnapshot>();
        var currentTime = DateTime.UtcNow;

        foreach (var item in response.Money)
        {
            var amount = mapper.Map<Money>(item);

            result.Add(new PositionSnapshot
            {
                PositionUid = string.Empty,
                AccountId = accountId,
                TimeUtc = currentTime,
                Quantity = amount.Value,
                Symbol = amount.Currency,
                InstrumentUid = string.Empty,
                InstrumentType = "Currency",
            });
        }

        foreach (var item in response.Securities)
        {
            result.Add(new PositionSnapshot
            {
                PositionUid = item.PositionUid,
                AccountId = accountId,
                TimeUtc = currentTime,
                Quantity = item.Balance,
                Symbol = string.Empty,
                InstrumentUid = item.InstrumentUid,
                InstrumentType = item.InstrumentType,
            });
        }

        foreach (var item in response.Futures)
        {
            result.Add(new PositionSnapshot
            {
                PositionUid = item.PositionUid,
                AccountId = accountId,
                TimeUtc = currentTime,
                Quantity = item.Balance,
                Symbol = string.Empty,
                InstrumentUid = item.InstrumentUid,
                InstrumentType = "Future",
            });
        }

        foreach (var item in response.Options)
        {
            result.Add(new PositionSnapshot
            {
                PositionUid = item.PositionUid,
                AccountId = accountId,
                TimeUtc = currentTime,
                Quantity = item.Balance,
                Symbol = string.Empty,
                InstrumentUid = item.InstrumentUid,
                InstrumentType = "Option",
            });
        }

        return result;
    }
}
