namespace Vertr.CommandLine.Models.BackTest;

public class BackTestResult
{
    public Dictionary<DateTime, Dictionary<string, object>> Items { get; } = [];

    public Dictionary<string, object>? FinalClosePositionsResult { get; set; }

    public Position[] Positions { get; set; } = [];
}