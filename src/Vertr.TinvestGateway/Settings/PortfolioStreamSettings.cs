namespace Vertr.TinvestGateway.Settings;

public class PortfolioStreamSettings
{
    public bool IsEnabled { get; set; }

    public string TopicKey { get; set; } = "Operations.Portfolio";
}
