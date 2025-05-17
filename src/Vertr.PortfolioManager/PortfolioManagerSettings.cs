namespace Vertr.PortfolioManager;

public class PortfolioManagerSettings
{
    public string TinvestGatewayUrl { get; set; } = string.Empty;

    public bool IsPortfolioConsumerEnabled { get; set; }

    public bool IsOrderOperationConsumerEnabled { get; set; }

    public string PortfoliosTopicKey { get; set; } = string.Empty;

    public string OperationsTopicKey { get; set; } = string.Empty;
}
