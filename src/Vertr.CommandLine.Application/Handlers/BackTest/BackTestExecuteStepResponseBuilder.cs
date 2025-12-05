using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.CommandLine.Models.Requests.Orders;

namespace Vertr.CommandLine.Application.Handlers.BackTest;

internal class BackTestExecuteStepResponseBuilder
{
    private readonly Dictionary<string, object> _items = [];

    private string? _message;

    private Exception? _exception;

    public BackTestExecuteStepResponseBuilder WithMessage(string message)
    {
        _message = message;
        _items[BackTestContextKeys.Message] = message;
        return this;
    }

    public BackTestExecuteStepResponseBuilder WithError(Exception? exception, string message)
    {
        _exception = exception;
        _message = message;
        _items[BackTestContextKeys.Message] = message;

        return this;
    }

    public BackTestExecuteStepResponseBuilder WithCandle(Candle? candle)
    {
        if (candle == null)
        {
            return this;
        }

        _items[BackTestContextKeys.LastCandle] = candle;
        return this;
    }

    public BackTestExecuteStepResponseBuilder WithMarketTime(DateTime marketTime)
    {
        _items[BackTestContextKeys.MarketTime] = marketTime;
        return this;
    }

    public BackTestExecuteStepResponseBuilder WithPredictedPrice(decimal price)
    {
        _items[BackTestContextKeys.PredictedPrice] = price;
        return this;
    }

    public BackTestExecuteStepResponseBuilder WithMarketPrice(decimal price)
    {
        _items[BackTestContextKeys.MarketPrice] = price;
        return this;
    }

    public BackTestExecuteStepResponseBuilder WithSignal(Direction signal)
    {
        _items[BackTestContextKeys.Signal] = signal;
        return this;
    }

    public BackTestExecuteStepResponseBuilder WithTrades(Trade[] trades)
    {
        _items[BackTestContextKeys.Trades] = trades;
        return this;
    }

    public BackTestExecuteStepResponseBuilder WithPositions(Position[] positions)
    {
        _items[BackTestContextKeys.Positions] = positions;
        return this;
    }

    public BackTestExecuteStepResponse Build()
    {
        return new BackTestExecuteStepResponse
        {
            Items = _items,
            Message = _message,
            Exception = _exception
        };
    }
}