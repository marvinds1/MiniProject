using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence.DatabaseContext;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDBContext>
{
    public ApplicationDBContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database")
            ?? "Server=localhost;Database=miniproject;User=design;Password=design";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
        optionsBuilder.UseMySql(connectionString, PersistenceRegistration.DefaultServerVersion);

        return new ApplicationDBContext(optionsBuilder.Options);
    }
}
