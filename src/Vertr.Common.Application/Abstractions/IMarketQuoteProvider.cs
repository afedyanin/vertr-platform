using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IMarketQuoteProvider
{
    public Quote? GetMarketQuote(Guid instrumentId);
}
