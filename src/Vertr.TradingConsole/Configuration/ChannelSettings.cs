namespace Vertr.TradingConsole.Configuration;

public record class ChannelSettings
{
    public string Channel { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
