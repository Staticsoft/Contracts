using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP.Server;

public class DependencyInjectionHttpRequestHandlerFactory(
    IServiceProvider provider
) : HttpEndpointFactory
{
    readonly IServiceProvider Provider = provider;

    public HttpEndpoint<RequestBody, ResponseBody> Resolve<RequestBody, ResponseBody>()
        => Provider.GetRequiredService<HttpEndpoint<RequestBody, ResponseBody>>();
}
