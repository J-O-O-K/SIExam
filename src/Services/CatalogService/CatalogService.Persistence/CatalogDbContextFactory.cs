using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CatalogService.Persistence;

public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();        
        
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=CatalogDb;User Id=sa;Password=P@ssword1;TrustServerCertificate=True;");

        return new CatalogDbContext(optionsBuilder.Options);
    }
}
