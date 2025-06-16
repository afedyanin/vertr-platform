using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.Internal;

internal class PortfolioManager : IPortfolioManager
{
    public Task<string[]> GetActiveAccounts()
        => Task.FromResult(new string[]
        {
          "a48c2760-20ae-4e0a-8d4b-4005cdb10d70",
          "f7c33024-67bc-428e-a149-e916e87e79ad",
          "0e284896-ba30-440f-9626-18ab2e2cc2f0"
        });
 
    public Task<PortfolioSnapshot?> GetLastPortfolio(string accountId, Guid? bookId = null)
    {
        throw new NotImplementedException();
    }

    public Task<PortfolioSnapshot?> GetPortfolioHistory(string accountId, Guid? bookId = null, int maxRecords = 100)
    {
        throw new NotImplementedException();
    }

    public Task<PortfolioSnapshot?> MakeSnapshot(string accountId, Guid? bookId = null)
    {
        throw new NotImplementedException();
    }
}
