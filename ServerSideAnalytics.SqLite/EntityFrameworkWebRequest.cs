using System;
using Maddalena;

namespace ServerSideAnalytics.SqLite
{
    public class EntityFrameworkWebRequest : IWebRequest
    {
        public long Id { get; set; }

        public DateTime Timestamp { get; set; }
        public string Identity { get; set; }
        public string RemoteIpAddress { get; set; }
        public string User { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string UserAgent { get; set; }
        public string Referer { get; set; }
        public CountryCode Country { get; set; }
    }
}
