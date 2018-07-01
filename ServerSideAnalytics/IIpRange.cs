using Maddalena;

namespace ServerSideAnalytics
{
    public interface IIpRange
    {
        CountryCode Country { get; set; }

        long TopFrom { get; set; }

        long TopTo { get; set; }

        long DownFrom { get; set; }

        long DownTo { get; set; }
    }
}