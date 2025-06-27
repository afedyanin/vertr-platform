using Microsoft.EntityFrameworkCore;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.DataAccess.Entities;

namespace Vertr.OrderExecution.DataAccess;

public class OrderExecutionDbContext : DbContext
{
    public DbSet<OrderEvent> OrderEvents { get; set; }

    public OrderExecutionDbContext(DbContextOptions<OrderExecutionDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new OrderEventEntityConfiguration().Configure(modelBuilder.Entity<OrderEvent>());
    }
}
