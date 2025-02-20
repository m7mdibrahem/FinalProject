using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CityCategory> CityCategories { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<RequestCompany> RequestCompanies { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RequestCompany>()
                .Property(e => e.CompanyStatus)
                .HasConversion<string>()
                .HasDefaultValue(CompStat.Pending);
            modelBuilder.Entity<Seat>()
                .Property(e => e.availability)
                .HasConversion<string>()
                .HasDefaultValue(Availability.Available);
            modelBuilder.Entity<Seat>()
               .Property(e => e.Degree)
               .HasConversion<string>();
            modelBuilder.Entity<Payment>()
                .Property(e => e.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Trip>()
                .Property(e => e.Status)
                .HasDefaultValue(tripStatus.Pending);
        }
    }
}
