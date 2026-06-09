using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Foodtrackr.Api.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<FoodPortion> FoodPortions { get; set; }
        public DbSet<FoodLogEntry> FoodLogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FoodItem>()
                .HasKey(f => f.FoodId);

            builder.Entity<FoodPortion>()
                .HasOne(p => p.FoodItem)
                .WithMany(f => f.Portions)
                .HasForeignKey(p => p.FoodId);

            builder.Entity<FoodLogEntry>()
                .HasOne(l => l.Patient)
                .WithMany()
                .HasForeignKey(l => l.PatientId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=dpg-d8k8msho3t8c73aq7e3g-a.singapore-postgres.render.com;Port=5432;Database=foodtrackr_db;Username=foodtrackr_db_user;Password=uI0YwO4Lw8XhvBiNraUp3gEqIUhB18sf;SSL Mode=Require;Trust Server Certificate=true");
            }
        }
    }
}