using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server
{
    public class DependencyInjectionParametrizedHttpRequestHandlerFactory : ParametrizedHttpEndpointFactory
    {
        readonly IServiceProvider Provider;

        public DependencyInjectionParametrizedHttpRequestHandlerFactory(IServiceProvider provider)
            => Provider = provider;

        public ParametrizedHttpEndpoint<RequestBody, ResponseBody> Resolve<RequestBody, ResponseBody>()
            => Provider.GetRequiredService<ParametrizedHttpEndpoint<RequestBody, ResponseBody>>();
    }
}
