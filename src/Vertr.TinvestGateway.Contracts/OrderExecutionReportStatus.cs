namespace Vertr.TinvestGateway.Contracts;

public enum OrderExecutionReportStatus
{
    Unspecified = 0,
    Fill = 1,
    Rejected = 2,
    Cancelled = 3,
    New = 4,
    Partiallyfill = 5,
}
