using System.Text;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Extensions;

public static class PortfolioExtensions
{
    public static string Dump(this Portfolio porftolio, string name, Instrument[] instruments)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Portfolio:{name} ID={porftolio.Id}");
        sb.Append($"Positions:[{porftolio.Positions.DumpPositions(instruments)}] ");
        sb.Append($"Commissions:[{porftolio.Comissions.DumpPositions(instruments)}]\n");

        return sb.ToString();
    }

    public static PortfolioInstrumentStats StatsByInstrument(this IEnumerable<Portfolio> porftolios, Guid instrumentId)
    {
        var positionsStats = porftolios
            .SelectMany(p => p.Positions)
            .Where(pos => pos.InstrumentId == instrumentId)
            .Select(p => p.Amount)
            .GetStats();

        var commissionsStats = porftolios
            .SelectMany(p => p.Comissions)
            .Where(pos => pos.InstrumentId == instrumentId)
            .Select(p => p.Amount)
            .GetStats();

        var stats = new PortfolioInstrumentStats
        {
            InstrumentId = instrumentId,
            PositionsStats = positionsStats,
            CommissionsStats = commissionsStats
        };

        return stats;
    }

    private static string DumpPositions(this IList<Position> positions, Instrument[] instruments)
    {
        var sb = new StringBuilder();

        foreach (var pos in positions)
        {
            var ticker = instruments.GetTicker(pos.InstrumentId);
            sb.Append($"{ticker}={pos.Amount};");
        }

        return sb.ToString().TrimEnd(';');
    }
}
