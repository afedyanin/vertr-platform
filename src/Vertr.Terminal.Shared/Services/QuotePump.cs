using Vertr.Terminal.Shared.Models;

namespace Vertr.Terminal.Shared.Services;

public class QuotePump : IQuotePump
{
    public event EventHandler<StockQuote> QuoteUpdate;

    private readonly List<StockQuote> _stockQuotes = QuotesSource.Init();

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
