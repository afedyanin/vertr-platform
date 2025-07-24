using Dapper;

namespace Vertr.Infrastructure.Sql.Tests;

internal class AuthorRepository : IAuthorRepository
{
    private CustomDataContext _context;
    private string databaseName = "demo.db";
    public AuthorRepository(CustomDataContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Author>> GetAll()
    {
        using var connection = await _context.CreateDatabase(databaseName);
        var sql = "SELECT * FROM Author";
        return await connection.QueryAsync<Author>(sql);
    }
    public async Task<Author> GetById(int id)
    {
        using var sqliteConnection = await
        _context.CreateDatabase(databaseName);
        string sql = "SELECT * FROM Author WHERE Id = @id";
        return await sqliteConnection.
        QueryFirstOrDefaultAsync<Author>(sql, new { id });
    }
    public async Task Create(Author Author)
    {
        using var sqliteConnection = await
        _context.CreateDatabase(databaseName);
        string sql = "INSERT INTO Author (FirstName, LastName, Address) " +
            "VALUES (@FirstName, @LastName, @Address)";
        await sqliteConnection.ExecuteAsync(sql, Author);
    }
    public async Task Update(Author Author)
    {
        using var sqliteConnection = await
        _context.CreateDatabase(databaseName);
        string sql = "UPDATE Author SET FirstName = @FirstName, " +
            "LastName = @LastName, Address = @Address WHERE Id = @Id";
        await sqliteConnection.ExecuteAsync(sql, Author);
    }
    public async Task Delete(int id)
    {
        using var sqliteConnection = await
        _context.CreateDatabase(databaseName);
        string sql = "DELETE FROM Author WHERE Id = @id";
        await sqliteConnection.ExecuteAsync(sql, new { id });
    }
}
