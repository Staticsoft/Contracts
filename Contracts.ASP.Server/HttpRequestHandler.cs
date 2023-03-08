using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server
{
    public interface HttpRequestHandler
    {
        Task Execute<RequestBody, ResponseBody>(HttpContext context, HttpEndpointMetadata metadata)
            where RequestBody : class, new()
            where ResponseBody : class, new();
    }
}
