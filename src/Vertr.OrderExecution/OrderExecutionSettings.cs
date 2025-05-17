namespace Vertr.OrderExecution;

public class OrderExecutionSettings
{
    public string TinvestGatewayUrl { get; set; } = string.Empty;

    public bool IsOperationsProducerEnabled { get; set; }
    public bool IsOrderStateConsumerEnabled { get; set; }
    public bool IsOrderTradesConsumerEnabled { get; set; }

    public string OperationsTopicKey { get; set; } = string.Empty;
    public string OrderStateTopicKey { get; set; } = string.Empty;
    public string OrderTradesTopicKey { get; set; } = string.Empty;
}
