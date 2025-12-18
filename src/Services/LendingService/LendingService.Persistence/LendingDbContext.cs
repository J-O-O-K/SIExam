using LendingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LendingService.Persistence;

public class LendingDbContext : DbContext
{
    public LendingDbContext(DbContextOptions<LendingDbContext> options) : base(options) { }

    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LendingDbContext).Assembly);
    }
}