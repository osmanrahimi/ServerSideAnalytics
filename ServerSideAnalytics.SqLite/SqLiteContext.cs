using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqLite
{
    class SqLiteContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<EntityFrameworkWebRequest> Requests { get; set; }

        public SqLiteContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}