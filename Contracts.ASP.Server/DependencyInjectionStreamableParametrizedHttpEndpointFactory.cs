using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server;

public class DependencyInjectionStreamableParametrizedHttpEndpointFactory : StreamableParametrizedHttpEndpointFactory
{
    readonly IServiceProvider Provider;

    public DependencyInjectionStreamableParametrizedHttpEndpointFactory(IServiceProvider provider)
        => Provider = provider;

    public StreamableParametrizedHttpEndpoint<RequestBody> Resolve<RequestBody>()
        => Provider.GetRequiredService<StreamableParametrizedHttpEndpoint<RequestBody>>();
}
