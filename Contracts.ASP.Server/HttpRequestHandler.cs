using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server
{
    public interface HttpRequestHandler
    {
        Task Execute<TRequest, TResponse>(HttpContext context);
    }
}
