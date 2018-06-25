using Microsoft.AspNetCore.Http;

namespace ServerSideAnalytics
{
    public interface IContextFilter
    {
        bool IsRelevant(HttpContext context);
    }
}
