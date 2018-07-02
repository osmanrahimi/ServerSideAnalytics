using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqLite
{
    internal class SqLiteContext : DbContext
    {
        private readonly string _table;
        private readonly string _connectionString;

        public SqLiteContext(string connectionString)
        {
            _connectionString = connectionString;
            _table = "WebRequest";
        }

        public SqLiteContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }

        public DbSet<SqliteWebRequest> WebRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SqliteWebRequest>(b => { b.ToTable(_table); });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}