using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server;

public class DependencyInjectionStreamableHttpEndpointFactory : StreamableHttpEndpointFactory
{
    readonly IServiceProvider Provider;

    public DependencyInjectionStreamableHttpEndpointFactory(IServiceProvider provider)
        => Provider = provider;

    public StreamableHttpEndpoint<RequestBody> Resolve<RequestBody>()
        => Provider.GetRequiredService<StreamableHttpEndpoint<RequestBody>>();
}
