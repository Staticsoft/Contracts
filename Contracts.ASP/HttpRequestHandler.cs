using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP
{
    public interface HttpRequestHandler<TRequest, TResponse>
    {
        Task Execute(HttpContext context);
    }

    public interface HttpRequestHandler
    {
        Task Execute<TRequest, TResponse>(HttpContext context);
    }
}
