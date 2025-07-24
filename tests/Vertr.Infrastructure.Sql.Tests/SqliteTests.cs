namespace Vertr.Infrastructure.Sql.Tests;
public class SqliteTests
{
    [Test]
    public async Task CanCreateDbConnection()
    {
        var context = new CustomDataContext();
        using var connection = context.CreateDatabaseConnection("demo.db");
        await connection.OpenAsync();
        await connection.CloseAsync();
    }

    [Test]
    public async Task CanCreateDb()
    {
        var context = new CustomDataContext();
        await context.CreateDatabase("demo.db");
    }

    [Test]
    public async Task CanCreateAuthor()
    {
        var context = new CustomDataContext();
        var repo = new AuthorRepository(context);

        var author = new Author
        {
            Address = "My Address",
            FirstName = "My First Name",
            LastName = "My Last Name"
        };

        await repo.Create(author);
    }
}
