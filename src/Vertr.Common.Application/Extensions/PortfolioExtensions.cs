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

    private static string DumpPositions(this IList<Position> positions, Instrument[] instruments)
    {
        var sb = new StringBuilder();

        foreach (var commission in positions)
        {
            var ticker = instruments.GetTicker(commission.InstrumentId);
            sb.Append($"{ticker}={commission.Amount};");
        }

        return sb.ToString().TrimEnd(';');
    }
}
