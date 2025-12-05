namespace Vertr.CommandLine.Models.Helpers;

public static class PositionExtensions
{
    public static PositionSummary GetSummary(this IEnumerable<Position>? positions, string currencyCode)
    {
        if (positions == null || !positions.Any())
        {
            return new PositionSummary();
        }

        var tradingPositionKey = Position.GetTradingPositionKey(currencyCode);
        var tradingAmount = decimal.Zero;

        var comissionPositionKey = Position.GetComissionsPositionKey(currencyCode);
        var comissions = decimal.Zero;

        var holdings = new Dictionary<string, decimal>();

        foreach (var position in positions)
        {
            if (position.Symbol == tradingPositionKey)
            {
                tradingAmount = position.Qty;
                continue;
            }

            if (position.Symbol == comissionPositionKey)
            {
                comissions = position.Qty;
                continue;
            }

            if (position.Qty != decimal.Zero)
            {
                holdings[position.Symbol] = position.Qty;
            }
        }

        return new PositionSummary
        {
            TradingAmount = tradingAmount,
            Comissions = comissions,
            Holdings = holdings,
        };
    }
}