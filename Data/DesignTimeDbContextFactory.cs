using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CantineAPI.Models;

namespace CantineAPI.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite("Data Source=cantine.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
