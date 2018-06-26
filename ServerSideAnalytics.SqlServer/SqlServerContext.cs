using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqlServer
{
    class SqlServerContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<EntityFrameworkWebRequest> Requests { get; set; }

        public SqlServerContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}