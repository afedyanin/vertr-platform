using Vertr.Terminal.Shared.Models;

namespace Vertr.Terminal.Shared.Services;

public interface IQuotePump
{
    event EventHandler<StockQuote> QuoteUpdate;

    Task RunAsync(CancellationToken stopToken);
}
