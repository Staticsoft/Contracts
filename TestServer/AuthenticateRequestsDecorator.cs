using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.ASP;
using Staticsoft.Contracts.ASP.Server;
using Staticsoft.TestContract;
using System.Linq;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class AuthenticateRequestsDecorator : HttpRequestHandler
    {
        readonly HttpRequestHandler Handler;

        public AuthenticateRequestsDecorator(HttpRequestHandler handler)
            => Handler = handler;

        public Task Execute<RequestBody, ResponseBody>(HttpContext context, HttpEndpointMetadata metadata)
            where RequestBody : class, new()
            where ResponseBody : class, new()
        {
            if (metadata.HasAttribute<AuthenticateRequestAttribute>())
            {
                var authentication = GetAuthentication(context);
                if (string.IsNullOrWhiteSpace(authentication))
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }
            }

            return Handler.Execute<RequestBody, ResponseBody>(context, metadata);
        }

        static string GetAuthentication(HttpContext context)
            => context.Request.Headers.TryGetValue("Authentication", out var values)
            ? values.Single()
            : string.Empty;

    }
}
