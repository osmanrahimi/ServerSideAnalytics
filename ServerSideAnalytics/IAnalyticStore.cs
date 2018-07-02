using System.Threading.Tasks;

namespace ServerSideAnalytics
{
    public interface IAnalyticStore
    {
        Task AddAsync(WebRequest request);
    }
}