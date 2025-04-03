using Vertr.Terminal.Shared.Models;

namespace Vertr.Terminal.Shared.Services;

public interface IQuoteHub
{
    Task SendQuoteInfo(StockQuote quote);

    Task QuoteHubMessage(string msg);
}
