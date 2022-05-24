using Microsoft.Extensions.DependencyInjection;
using System;

namespace Staticsoft.Contracts.ASP.Server
{
    public class DependencyInjectionHttpRequestHandlerFactory : HttpRequestHandlerFactory
    {
        readonly IServiceProvider Provider;

        public DependencyInjectionHttpRequestHandlerFactory(IServiceProvider provider)
            => Provider = provider;

        public HttpRequestHandler<TRequest, TResponse> Create<TRequest, TResponse>()
            => Provider.GetRequiredService<HttpRequestHandler<TRequest, TResponse>>();
    }
}
