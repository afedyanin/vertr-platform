namespace Vertr.TinvestGateway.Settings;

public class OrderTradesStreamSettings
{
    public bool IsEnabled { get; set; }

    public string TopicKey { get; set; } = "Orders.OrderTrades";
}
