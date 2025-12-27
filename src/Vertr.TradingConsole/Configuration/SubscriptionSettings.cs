namespace Vertr.TradingConsole.Configuration;

public record class SubscriptionSettings
{
    public ChannelSettings Portfolios { get; set; } = new ChannelSettings();

    public ChannelSettings OrderBooks { get; set; } = new ChannelSettings();

    public ChannelSettings Candles { get; set; } = new ChannelSettings();
}
