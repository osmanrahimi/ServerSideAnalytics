using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqlServer
{
    internal class SqlServerContext : DbContext
    {
        private readonly string _requestTable;
        private readonly string _geoIpTable;
        private readonly string _connectionString;

        public SqlServerContext(string connectionString, string requestTable, string geoIpTable)
        {
            _connectionString = connectionString;
            _requestTable = requestTable;
            _geoIpTable = geoIpTable;
        }

        public DbSet<SqlServerWebRequest> WebRequest { get; set; }

        public DbSet<SqlServerGeoIpRange> GeoIpRange { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SqlServerWebRequest>(b => { b.ToTable(_requestTable); });
            modelBuilder.Entity<SqlServerGeoIpRange>(b => { b.ToTable(_geoIpTable); });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}