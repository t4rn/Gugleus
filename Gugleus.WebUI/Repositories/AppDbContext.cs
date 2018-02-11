using Gugleus.Core.Domain.Requests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gugleus.WebUI.Repositories
{
    public class AppDbContext : DbContext
    {
        private string _cs;

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("he");
            //modelBuilder.Entity<Request>()
            //    .ToTable("requests", schema: "he")
            //    .Property(x => x.Id).HasColumnName("id");
        }

        internal void SetConnectionString(string cs)
        {
            _cs = cs;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _cs;
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
