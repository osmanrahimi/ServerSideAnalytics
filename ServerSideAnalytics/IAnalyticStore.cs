using System.Threading.Tasks;

namespace ServerSideAnalytics
{
    public interface IAnalyticStore<T> where T : IWebRequest
    {
        T GetNew();
        Task AddAsync(T request);
    }
}