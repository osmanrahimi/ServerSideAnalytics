﻿using Maddalena;

namespace ServerSideAnalytics.SqlServer
{
    internal class SqlServerGeoIpRange
    {
        public long Id { get; set; }

        public long FromUp { get; set; }
        public long FromDown { get; set; }

        public long ToUp { get; set; }
        public long ToDown { get; set; }

        public CountryCode CountryCode { get; set; }
    }
}
