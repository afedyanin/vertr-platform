using Vertr.Common.Contracts;

namespace Vertr.Common.DataAccess.Repositories;

public interface IOpenInterestRepository
{
    public Task Clear();
    public Task<IEnumerable<OpenInterest>> GetAll();
    public Task<OpenInterest?> Get(Guid instrumentId);
    public Task Save(OpenInterest openInterest);

}
