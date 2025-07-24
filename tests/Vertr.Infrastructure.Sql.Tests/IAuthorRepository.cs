namespace Vertr.Infrastructure.Sql.Tests;
public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAll();
    Task<Author> GetById(int id);
    Task Create(Author Author);
    Task Update(Author Author);
    Task Delete(int id);
}
