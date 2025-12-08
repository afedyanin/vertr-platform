using System.Text.RegularExpressions;

namespace Vertr.Common.Tests;

public class RedisChannelNameParserTests
{
    private const string GuidRegexPattern = @"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})";
    public static readonly Regex GuidRegex = new Regex(GuidRegexPattern, RegexOptions.Compiled);

    [TestCase("market.candles.e6123145-9665-43e0-8413-cd61b8aa9b13", "e6123145-9665-43e0-8413-cd61b8aa9b13")]
    [TestCase("market.candles.e6123145-9665-43e0-8413-cd61b8aa9b13.345", "e6123145-9665-43e0-8413-cd61b8aa9b13")]
    public void CanGetInstrumentIdFromRedisChannel(string channelName, string guidString)
    {
        var res = GuidRegex.Match(channelName)?.Value;
        Assert.That(res, Is.EqualTo(guidString));
    }
}
