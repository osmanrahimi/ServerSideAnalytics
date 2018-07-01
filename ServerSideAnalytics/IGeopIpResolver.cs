using Maddalena;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideAnalytics
{
    public interface IGeopIpResolver<T> where T : IIpRange
    {
        T GetNew();

        Task StoreAsync(T range);

        Task<CountryCode> Resolve(T range);
    }
}
