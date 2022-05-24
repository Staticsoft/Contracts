using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.ASP;
using Staticsoft.Contracts.ASP.Server;
using Staticsoft.TestContract;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class AuthenticateRequestsDecorator : HttpRequestHandler
    {
        readonly HttpRequestHandler Handler;
        readonly IServiceProvider Provider;

        public AuthenticateRequestsDecorator(HttpRequestHandler handler, IServiceProvider provider)
        {
            Handler = handler;
            Provider = provider;
        }

        public Task Execute<TRequest, TResponse>(HttpContext context)
        {
            var metadata = Provider.GetRequiredService<HttpEndpointMetadata<TRequest, TResponse>>();
            if (metadata.HasAttribute<AuthenticateRequestAttribute>())
            {
                var authentication = GetAuthentication(context);
                if (string.IsNullOrWhiteSpace(authentication))
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }
            }

            return Handler.Execute<TRequest, TResponse>(context);
        }

        static string GetAuthentication(HttpContext context)
            => context.Request.Headers.TryGetValue("Authentication", out var values)
            ? values.Single()
            : string.Empty;

    }
}
