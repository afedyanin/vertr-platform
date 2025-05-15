namespace Vertr.PortfolioManager;

public class PortfolioManagerSettings
{
    public string TinvestGatewayUrl { get; set; } = string.Empty;

    public bool IsPortfolioConsumerEnabled { get; set; }

    public string PortfolioConsumerTopicKey { get; set; } = string.Empty;
}
