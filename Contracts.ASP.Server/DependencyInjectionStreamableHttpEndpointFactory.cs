using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server;

public class DependencyInjectionStreamableHttpEndpointFactory(
    IServiceProvider provider
) : StreamableHttpEndpointFactory
{
    readonly IServiceProvider Provider = provider;

    public StreamableHttpEndpoint<RequestBody> Resolve<RequestBody>()
        => Provider.GetRequiredService<StreamableHttpEndpoint<RequestBody>>();
}
