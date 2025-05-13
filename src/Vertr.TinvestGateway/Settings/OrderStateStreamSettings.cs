namespace Vertr.TinvestGateway.Settings;

public class OrderStateStreamSettings
{
    public bool IsEnabled { get; set; }

    public string TopicKey { get; set; } = "Orders.OrderState";
}
