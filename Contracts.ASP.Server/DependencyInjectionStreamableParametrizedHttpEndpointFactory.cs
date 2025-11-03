using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server;

public class DependencyInjectionStreamableParametrizedHttpEndpointFactory(
    IServiceProvider provider
) : StreamableParametrizedHttpEndpointFactory
{
    readonly IServiceProvider Provider = provider;

    public StreamableParametrizedHttpEndpoint<RequestBody> Resolve<RequestBody>()
        => Provider.GetRequiredService<StreamableParametrizedHttpEndpoint<RequestBody>>();
}
