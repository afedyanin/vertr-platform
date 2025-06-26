using System.Text.Json;
using StackExchange.Redis;
using Vertr.MarketData.Contracts;

namespace Vertr.Paltform.Tests;
public class MarketDataTests : ApplicationTestBase
{
    private ConnectionMultiplexer? _redis;

    private static readonly string[] _symbols = [
        "AFKS",
        "GAZP",
        "GMKN",
        "LKOH",
        "MGNT",
        "MOEX",
        "NLMK",
        "OZON",
        "ROSN",
        "SBER",
        "SBERP",
        "T",
        "VTBR",
        "X5",
    ];

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _redis = ConnectionMultiplexer.Connect("localhost");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (_redis != null)
        {
            _redis.Dispose();
            _redis = null;
        }
    }

    [Test]
    public async Task CanGetInstruments()
    {
        var instruments = await GetInstruments(_symbols);

        Assert.That(instruments, Is.Not.Null);

        var hash = new List<HashEntry>();

        foreach (var item in instruments)
        {
            var value = new RedisValue(JsonSerializer.Serialize(item));
            var entry = new HashEntry(GetKey(item), value);

            hash.Add(entry);
        }

        var db = _redis!.GetDatabase();
        db.HashSet("mds:isntruments", [.. hash]);

        var hashFields = db.HashGetAll("mds:isntruments");

        foreach (var field in hashFields)
        {
            Console.WriteLine(field);
        }
    }

    [Test]
    public async Task CanGetInstrumentFromRedis()
    {
        var db = _redis!.GetDatabase();
        var sber = await db.HashGetAsync("mds:isntruments", "TQBR:SBER");

        Assert.That(sber.HasValue, Is.True);

        Console.WriteLine(sber.ToString());
    }

    [Test]
    public async Task CanDeleteKey()
    {
        var db = _redis!.GetDatabase();
        var deleted = await db.KeyDeleteAsync("mds:isntruments");
        Assert.That(deleted, Is.True);
    }

    private static string GetKey(Instrument instrument)
        => $"{instrument.InstrumentIdentity.ClassCode}:{instrument.InstrumentIdentity.Ticker}";

    private async Task<Instrument[]?> GetInstruments(string[] symbols)
    {
        var res = new List<Instrument>();

        foreach (var symbol in symbols)
        {
            var instrument = await GetInstrument(symbol);
            if (instrument != null)
            {
                res.Add(instrument);
            }
        }

        return [.. res];
    }

    private async Task<Instrument?> GetInstrument(string symbol)
        => await VertrClient.GetInstrumentByTicker("TQBR", symbol);
}
