using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Application.Services;

internal class PortfolioService : IPortfolioService
{
    private class Portfolio
    {
        public Guid Id { get; init; }

        public Dictionary<string, Position> Positions { get; init; } = [];

        public Position GetOrCreatePosition(string symbol)
        {
            if (Positions.TryGetValue(symbol, out var position))
            {
                return position;
            }

            position = new Position { PortfolioId = Id, Symbol = symbol, Qty = decimal.Zero };
            Positions.Add(symbol, position);
            return position;
        }
    }

    private readonly Dictionary<Guid, Portfolio> _portfolios = [];

    public Position[] Update(Guid portfolioId, string symbol, Trade[] trades, string currencyCode)
    {
        _portfolios.TryGetValue(portfolioId, out var portfolio);

        if (portfolio == null)
        {
            portfolio = new Portfolio()
            {
                Id = portfolioId,
            };

            _portfolios[portfolioId] = portfolio;
        }

        var currencyPosition = portfolio.GetOrCreatePosition(Position.GetTradingPositionKey(currencyCode));
        var comissionsPosition = portfolio.GetOrCreatePosition(Position.GetComissionsPositionKey(currencyCode));
        var tradingPosition = portfolio.GetOrCreatePosition(symbol);


        foreach (var trade in trades)
        {
            tradingPosition.Qty += trade.Quantity;
            currencyPosition.Qty -= trade.Quantity * trade.Price;
            comissionsPosition.Qty -= trade.Comission;
        }

        return GetPositions(portfolioId);
    }

    public Position[] GetPositions(Guid portfolioId)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            return [.. portfolio.Positions.Values.Select(p => p.ClonePosition())];
        }

        return [];
    }

    public Position? GetPosition(Guid portfolioId, string symbol)
    {
        if (!_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            return null;
        }

        portfolio.Positions.TryGetValue(symbol, out var position);

        return position?.ClonePosition();
    }
}