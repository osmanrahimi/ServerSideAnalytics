using Microsoft.EntityFrameworkCore;

namespace ServerSideAnalytics.SqLite
{
    internal class SqLiteContext : DbContext
    {
        private readonly string _requestTable;
        private readonly string _geoIpTable;
        private readonly string _connectionString;

        public SqLiteContext(string connectionString)
        {
            _connectionString = connectionString;
            _requestTable = "WebRequest";
            _geoIpTable = "GeoIp";
        }

        public SqLiteContext(string connectionString, string requestTable, string geoIpTable)
        {
            _connectionString = connectionString;
            _requestTable = requestTable;
            _geoIpTable = geoIpTable;
        }

        public DbSet<SqliteWebRequest> WebRequest { get; set; }

        public DbSet<SqLiteGeoIpRange> GeoIpRange { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SqliteWebRequest>(b => { b.ToTable(_requestTable); });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}