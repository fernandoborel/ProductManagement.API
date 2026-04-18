using Microsoft.EntityFrameworkCore;
using ProductManagement.Infra.Data.SqlServer.Entities;

namespace ProductManagement.Infra.Data.SqlServer.Contexts;

public class SqlServerContext : DbContext
{
    public SqlServerContext(DbContextOptions<SqlServerContext> options)
        : base(options){}

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("PRODUCTS");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Price).HasPrecision(18, 2);
        });
    }
}