using Gugleus.Core.Domain.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Gugleus.WebUI.Repositories
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        private string _cs;
        private readonly IConfiguration _config;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("he");
            //modelBuilder.Entity<Request>()
            //    .ToTable("requests", schema: "he")
            //    .Property(x => x.Id).HasColumnName("id");
            base.OnModelCreating(modelBuilder);
        }

        internal void SetConnectionString(string cs)
        {
            _cs = cs;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _cs;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = _config.GetConnectionString("csErexus");
            }

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
