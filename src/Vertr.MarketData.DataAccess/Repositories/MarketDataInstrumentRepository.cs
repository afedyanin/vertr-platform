using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.DataAccess.Repositories;

internal class MarketDataInstrumentRepository : RepositoryBase, IMarketDataInstrumentRepository
{
    public MarketDataInstrumentRepository(IDbContextFactory<MarketDataDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<Instrument[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .Instruments
            .ToArrayAsync();
    }

    public async Task<Instrument?> GetById(Guid instrumentId)
    {
        using var context = await GetDbContext();

        return await context
            .Instruments
            .SingleOrDefaultAsync(x => x.Id == instrumentId);
    }

    public async Task<Instrument?> GetBySymbol(Symbol symbol)
    {
        using var context = await GetDbContext();

        return await context
            .Instruments
            .SingleOrDefaultAsync(x => x.Symbol == symbol);
    }

    public async Task<bool> Save(Instrument instrument)
    {
        using var context = await GetDbContext();

        var existing = await context
            .Instruments
            .FirstOrDefaultAsync(p => p.Id == instrument.Id);

        if (existing != null)
        {
            existing.Currency = instrument.Currency;
            existing.InstrumentKind = instrument.InstrumentKind;
            existing.InstrumentType = instrument.InstrumentType;
            existing.LotSize = instrument.LotSize;
            existing.Name = instrument.Name;
            existing.Symbol = instrument.Symbol;
        }
        else
        {
            context.Instruments.Add(instrument);
        }

        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(Guid Id)
    {
        using var context = await GetDbContext();

        return await context.Instruments
            .Where(s => s.Id == Id)
            .ExecuteDeleteAsync();
    }
}

