using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Tests.Repositories;

[TestFixture(Category = "Database", Explicit = true)]
public class MarketDataInstrumentRepositoryTests : RepositoryTestBase
{
    [Test]
    public async Task CanGetInstruments()
    {
        var items = await InstrumentsRepo.GetAll();
        Assert.That(items, Is.Not.Null);
    }

    [TestCase("f85b99c2-4e21-41c4-a4fe-52ef5b1a0047")]
    public async Task CanSaveInstrument(string instrumentId)
    {
        var guid = Guid.Parse(instrumentId);
        var instrument = new Instrument
        {
            Id = guid,
            Symbol = new Symbol("CCC", "TTT"),
            Currency = "rub",
            Name = "Test instrument",
            LotSize = 1,
        };

        var saved = await InstrumentsRepo.Save(instrument);

        Assert.That(saved, Is.True);
    }

    [TestCase("f85b99c2-4e21-41c4-a4fe-52ef5b1a0047")]
    public async Task CanGetInstrumentById(string instrumentId)
    {
        var guid = Guid.Parse(instrumentId);
        var item = await InstrumentsRepo.GetById(guid);
        Assert.That(item, Is.Not.Null);
    }

    [TestCase("CCC", "TTT")]
    public async Task CanGetInstrumentByTicker(string classCode, string ticker)
    {
        var sym = new Symbol(classCode, ticker);
        var item = await InstrumentsRepo.GetBySymbol(sym);
        Assert.That(item, Is.Not.Null);
    }

    [TestCase("f85b99c2-4e21-41c4-a4fe-52ef5b1a0047")]
    public async Task CanUpdateInstrument(string instrumentId)
    {
        var guid = Guid.Parse(instrumentId);
        var item = await InstrumentsRepo.GetById(guid);
        Assert.That(item, Is.Not.Null);

        var newItem = new Instrument
        {
            Id = item.Id,
            Symbol = item.Symbol,
            Currency = item.Currency,
            Name = item.Name + " (Updated)",
            LotSize = item.LotSize,
        };

        var saved = await InstrumentsRepo.Save(newItem);

        Assert.That(saved, Is.True);
    }

    [TestCase("f85b99c2-4e21-41c4-a4fe-52ef5b1a0047")]
    public async Task CanDeleteInstrument(string instrumentId)
    {
        var guid = Guid.Parse(instrumentId);
        var deleted = await InstrumentsRepo.Delete(guid);
        Assert.That(deleted, Is.EqualTo(1));
    }
}
