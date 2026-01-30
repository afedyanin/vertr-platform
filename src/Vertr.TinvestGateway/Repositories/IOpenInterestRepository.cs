using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway.Repositories;

public interface IOpenInterestRepository
{
    public Task Clear();
    public Task<IEnumerable<OpenInterest>> GetAll();
    public Task<OpenInterest?> Get(Guid instrumentId);
    public Task Save(OpenInterest openInterest);

}
