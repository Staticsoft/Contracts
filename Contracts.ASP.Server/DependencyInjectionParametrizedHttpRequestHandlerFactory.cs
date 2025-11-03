using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server;

public class DependencyInjectionParametrizedHttpRequestHandlerFactory(
    IServiceProvider provider
) : ParametrizedHttpEndpointFactory
{
    readonly IServiceProvider Provider = provider;

    public ParametrizedHttpEndpoint<RequestBody, ResponseBody> Resolve<RequestBody, ResponseBody>()
        => Provider.GetRequiredService<ParametrizedHttpEndpoint<RequestBody, ResponseBody>>();
}
