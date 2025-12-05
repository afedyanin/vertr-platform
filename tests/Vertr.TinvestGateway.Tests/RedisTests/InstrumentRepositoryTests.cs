using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.DataAccess.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests;

public class InstrumentRepositoryTests
{
    private IConnectionMultiplexer _connectionMultiplexer;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _connectionMultiplexer.Close();
    }

    [TearDown]
    public async Task TearDown()
    {
        var repo = new InstrumentRepository(_connectionMultiplexer);
        await repo.Clear();
    }


    [Test]
    public async Task CanSaveInstrument()
    {
        var repo = new InstrumentRepository(_connectionMultiplexer);
        var instrument = CreateInstrument();

        await repo.Save(instrument);
        var saved = await repo.Get(instrument.Id);
        Assert.That(saved, Is.Not.Null);

        Console.WriteLine(saved);
    }

    [Test]
    public async Task CanChangeInstrumentDetails()
    {
        var repo = new InstrumentRepository(_connectionMultiplexer);
        var instrument = CreateInstrument();
        await repo.Save(instrument);

        instrument.LotSize = 987;
        await repo.Save(instrument);

        var item = await repo.Get(instrument.Id);

        Assert.That(item, Is.Not.Null);
        Assert.That(item.LotSize, Is.EqualTo(instrument.LotSize));

        Console.WriteLine(item);
    }

    private static Instrument CreateInstrument()
        => new Instrument
        {
            ClassCode = "ClassCode",
            Ticker = "Ticker",
            Currency = "RUB",
            Id = Guid.NewGuid(),
            InstrumentType = "SomeType",
            LotSize = 10,
            Name = "Some instrument name"
        };
}