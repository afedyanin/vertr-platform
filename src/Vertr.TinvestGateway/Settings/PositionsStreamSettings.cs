namespace Vertr.TinvestGateway.Settings;

public class PositionsStreamSettings
{
    public bool IsEnabled { get; set; }

    public string TopicKey { get; set; } = "Operations.Positions";
}
