using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server
{
    public class DependencyInjectionHttpRequestHandlerFactory : HttpEndpointFactory
    {
        readonly IServiceProvider Provider;

        public DependencyInjectionHttpRequestHandlerFactory(IServiceProvider provider)
            => Provider = provider;

        public HttpEndpoint<TRequest, TResponse> Resolve<TRequest, TResponse>()
            => Provider.GetRequiredService<HttpEndpoint<TRequest, TResponse>>();
    }
}
