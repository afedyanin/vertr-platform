namespace Vertr.OrderExecution.Application;
public class OrderExecutionSettings
{
    public string AccountId { get; set; } = string.Empty;

    public decimal Comission { get; set; } = 0.03m;

    public bool IsPaperTrading { get; set; }
}
