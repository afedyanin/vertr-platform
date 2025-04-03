using Vertr.Terminal.Shared.Models;

namespace Vertr.Terminal.Shared.Services;

public class QuotePump : IQuotePump
{
    public event EventHandler<StockQuote> QuoteUpdate;

    private List<StockQuote> _stockQuotes = [];

    public QuotePump()
    {
        Init();
    }

    private void Init()
    {
        _stockQuotes.Add(new StockQuote() { Symbol = "MSFT", Price = 80, BasePrice = 80, Volume = 2000, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "AAPL", Price = 100, BasePrice = 100, Volume = 3000, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "TSLA", Price = 350, BasePrice = 350, Volume = 120, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "NVDA", Price = 420, BasePrice = 420, Volume = 400, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "GOOG", Price = 1640, BasePrice = 1640, Volume = 110, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "AMZN", Price = 3200, BasePrice = 3200, Volume = 99, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "710000", Price = 34.845m, BasePrice = 34.845m, Volume = 3990203, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "703712", Price = 28.73m, BasePrice = 28.73m, Volume = 4972549, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "823212", Price = 6.892m, BasePrice = 6.892m, Volume = 9374495, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "604700", Price = 44.54m, BasePrice = 44.54m, Volume = 1052172, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "ENAG99", Price = 8.816m, BasePrice = 8.816m, Volume = 8198277, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "A0WMPJ", Price = 8.87m, BasePrice = 8.87m, Volume = 1715021, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "555750", Price = 12.655m, BasePrice = 12.655m, Volume = 16028358, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "716460", Price = 90.18m, BasePrice = 90.18m, Volume = 6439388, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "581005", Price = 125.3m, BasePrice = 125.3m, Volume = 753801, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "BASF11", Price = 46.31m, BasePrice = 46.31m, Volume = 3644131, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "BAY001", Price = 40.005m, BasePrice = 40.005m, Volume = 6964005, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "843002", Price = 196.75m, BasePrice = 196.75m, Volume = 808543, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "555200", Price = 29.07m, BasePrice = 29.07m, Volume = 4172211, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "578560", Price = 31.24m, BasePrice = 31.24m, Volume = 12094230, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "520000", Price = 88.94m, BasePrice = 88.94m, Volume = 408959, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "606214", Price = 31.98m, BasePrice = 31.98m, Volume = 1767693, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "A0D9PT", Price = 130, BasePrice = 130, Volume = 254502, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "604843", Price = 78.84m, BasePrice = 78.84m, Volume = 800386, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "A2DSYC", Price = 173.65m, BasePrice = 173.65m, Volume = 1248213, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "578580", Price = 64.86m, BasePrice = 64.86m, Volume = 1189784, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "A1EWWW", Price = 224.1m, BasePrice = 224.1m, Volume = 747668, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "A1ML7J", Price = 52.4m, BasePrice = 52.4m, Volume = 14648193, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "766403", Price = 123.42m, BasePrice = 123.42m, Volume = 1124661, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "846900", Price = 11463.9m, BasePrice = 11463.9m, Volume = 100607290, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "623100", Price = 18.936m, BasePrice = 18.936m, Volume = 4419546, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "519000", Price = 54.39m, BasePrice = 54.39m, Volume = 1694979, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "840400", Price = 149.32m, BasePrice = 149.32m, Volume = 1288010, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "703000", Price = 61.58m, BasePrice = 61.58m, Volume = 178345, Time = DateTime.Now });
        _stockQuotes.Add(new StockQuote() { Symbol = "543900", Price = 81.72m, BasePrice = 81.72m, Volume = 450792, Time = DateTime.Now });

    }

    public async Task RunAsync(CancellationToken stopToken)
    {
        int i = 0;

        Random rndPrice = new Random();
        Random rndVol = new Random();
        Random rndIdx = new Random();

        while (!stopToken.IsCancellationRequested)
        {
            i = (int)(rndIdx.NextDouble() * _stockQuotes.Count);

            if (i < _stockQuotes.Count)
            {
                var quote = _stockQuotes[i];

                var p = (decimal)((double)quote.Price * (((rndPrice.NextDouble() - 0.48d) / 1000d) + 1d));
                quote.Price = Math.Round(p, 2);
                quote.Volume = (int)(rndVol.NextDouble() * 3000d / (double)p);
                quote.Time = DateTime.Now;

                QuoteUpdate?.Invoke(this, quote);
            }

            await Task.Delay(50);
        }
    }
}
