using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Vertr.OrderExecution.DataAccess;
using System.Reflection;

namespace Vertr.Infrastructure.Pgsql.Migrations.Contexts;

public class OrderExecutionDbContextFactory : IDesignTimeDbContextFactory<OrderExecutionDbContext>
{
    public OrderExecutionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderExecutionDbContext>();

        optionsBuilder.UseNpgsql(
            ConnectionStrings.LocalConnection, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new OrderExecutionDbContext(optionsBuilder.Options);
    }
}


