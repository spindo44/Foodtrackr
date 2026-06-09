using Foodtrackr.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Foodtrackr.Api
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=dpg-d8ka6506dvec73fp08dg-a.singapore-postgres.render.com;Port=5432;Database=foodtrackr_db_pjqa;Username=foodtrackr_db_pjqa_user;Password=oMYBbKVYPnQM7SXeDNWUBHO4ZHBdZMOF;SSL Mode=Require;Trust Server Certificate=true");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}