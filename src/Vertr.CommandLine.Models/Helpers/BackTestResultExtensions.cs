using System.Text;
using Vertr.CommandLine.Models.BackTest;
using Vertr.CommandLine.Models.Requests.BackTest;

namespace Vertr.CommandLine.Models.Helpers;

public static class BackTestResultExtensions
{
    public static PositionSummary GetSummary(this BackTestResult backTestResult, string currencyCode)
        => backTestResult.Positions.GetSummary(currencyCode);

    public static string DumpLastStep(this BackTestResult? backTestResult)
    {
        return backTestResult == null ? string.Empty : backTestResult.DumpStep(backTestResult.Items.Keys.Order().Last());
    }

    public static IEnumerable<string> DumpAll(this BackTestResult? backTestResult)
    {
        if (backTestResult == null)
        {
            return [];
        }

        var res = new List<string>();

        foreach (var item in backTestResult.Items)
        {
            res.Add(DumpItems(item.Value));
        }

        return res;
    }

    public static string DumpStep(this BackTestResult? backTestResult, DateTime timeStep)
    {
        if (backTestResult == null)
        {
            return string.Empty;
        }

        backTestResult.Items.TryGetValue(timeStep, out var items);
        return DumpItems(items);
    }

    public static string DumpCloseStep(this BackTestResult? backTestResult)
    {
        return DumpItems(backTestResult?.FinalClosePositionsResult ?? []);
    }

    internal static string DumpItems(IDictionary<string, object>? items)
    {
        if (items == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        foreach (var item in items)
        {
            if (item.Key == BackTestContextKeys.Trades)
            {
                if (item.Value is not Trade[] trades || trades.Length == 0)
                {
                    continue;
                }

                sb.AppendLine("Trades:");

                foreach (var trade in trades)
                {
                    sb.AppendLine($"\t{trade}");
                }

                continue;
            }

            if (item.Key == BackTestContextKeys.Positions)
            {
                if (item.Value is not Position[] positions || positions.Length == 0)
                {
                    continue;
                }

                sb.AppendLine("Positions:");

                foreach (var position in positions)
                {
                    sb.AppendLine($"\t{position}");
                }

                continue;
            }

            sb.AppendLine($"{item.Key}={item.Value}");
        }

        return sb.ToString();
    }
}