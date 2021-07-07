
using Microsoft.EntityFrameworkCore;

namespace UniqueVisitorsCodingChallenge.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<UniqueVisitor> UniqueVisitors { get; set; }

        public ApplicationContext(string connectionSting)
        {
            _connectionString = connectionSting;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString);
        }
    }
}
