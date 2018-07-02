using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqlServer
{
    internal class SqlServerContext : DbContext
    {
        private readonly string _table;
        private readonly string _connectionString;

        public SqlServerContext(string connectionString)
        {
            _connectionString = connectionString;
            _table = "WebRequest";
        }

        public SqlServerContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }

        public DbSet<SqlServerWebRequest> WebRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SqlServerWebRequest>(b => { b.ToTable(_table); });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}