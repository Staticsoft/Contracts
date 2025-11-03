using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server;

public interface StreamableHttpRequestHandler
{
    Task Execute<RequestBody>(HttpContext context, StreamableHttpEndpointMetadata metadata)
        where RequestBody : class, new();
}