namespace Vertr.Backtest.Contracts;
public class BackTestExecutionResult
{
    public Guid BackTestId { get; set; }

    public BackTestState State { get; set; }

    public string? Message { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? StoppedAt { get; set; }

    public DateTime? LastCandleTime { get; set; }
}
