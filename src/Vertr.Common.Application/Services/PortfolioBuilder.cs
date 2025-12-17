using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

public class PortfolioBuilder
{
    private readonly Guid _portfolioId;
    private readonly Dictionary<Guid, Position> _comissions = [];
    private readonly Dictionary<Guid, Position> _positions = [];
    private readonly Dictionary<string, Instrument> _instrumentsByTicker;

    public PortfolioBuilder(
        Portfolio portfolio,
        IEnumerable<Instrument> instruments)
        : this(portfolio.Id, instruments)
    {
        _positions = portfolio.Positions.ToDictionary(x => x.InstrumentId, x => x);
        _comissions = portfolio.Comissions.ToDictionary(x => x.InstrumentId, x => x);
    }
    public PortfolioBuilder(
        Guid portfolioId,
        IEnumerable<Instrument> instruments)
    {
        _portfolioId = portfolioId;
        _instrumentsByTicker = InstrumentsByTicker(instruments);
    }

    public PortfolioBuilder ApplyComission(decimal amount, string currency)
    {
        _instrumentsByTicker.TryGetValue(currency, out var instrument);
        var key = instrument == null ? Guid.Empty : instrument.Id;

        _comissions.TryGetValue(key, out var comissionEntry);

        _comissions[key] = new Position
        {
            InstrumentId = key,
            Amount = comissionEntry.Amount + amount,
        };

        return this;
    }

    public PortfolioBuilder ApplyTrades(Guid instrumentId, Trade[] trades, TradingDirection direction)
    {
        foreach (var trade in trades)
        {
            ApplyTrade(trade, instrumentId, direction);
        }

        return this;
    }

    public Portfolio Build()
    {
        var portfolio = new Portfolio()
        {
            Id = _portfolioId,
            UpdatedAt = DateTime.UtcNow,
            Comissions = [.. _comissions.Values],
            Positions = [.. _positions.Values],
        };

        return portfolio;
    }

    private void ApplyTrade(Trade trade, Guid instrumentId, TradingDirection direction)
    {
        var qtySign = direction == TradingDirection.Sell ? -1 : 1;
        var price = trade.Price;

        _instrumentsByTicker.TryGetValue(trade.Currency, out var currencyInstrument);
        var currencyId = currencyInstrument?.Id ?? Guid.Empty;

        _positions.TryGetValue(instrumentId, out var positionEntry);
        _positions.TryGetValue(currencyId, out var moneyPositionEntry);

        _positions[instrumentId] = new Position
        {
            InstrumentId = instrumentId,
            Amount = positionEntry.Amount + trade.Quantity * qtySign
        };

        _positions[currencyId] = new Position
        {
            InstrumentId = currencyId,
            Amount = moneyPositionEntry.Amount + price * trade.Quantity * qtySign * (-1)
        };
    }

    private Dictionary<string, Instrument> InstrumentsByTicker(IEnumerable<Instrument> instruments)
        => instruments.ToDictionary(x => x.Ticker, x => x, StringComparer.OrdinalIgnoreCase);
}
