namespace Vertr.CommandLine.Models.Abstracttions;

public interface IOrderExecutionService
{
    public Task<Trade[]> PostOrder(
        string symbol,
        decimal qty,
        decimal price,
        decimal comissionPercent);
}