using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer;

public class EmptyEndpointHandlerProxy(
    HttpEndpoint<EmptyRequest, EmptyResponse> endpoint
) : HttpEndpoint<EmptyRequestProxy, EmptyResponse>
{
    readonly HttpEndpoint<EmptyRequest, EmptyResponse> Endpoint = endpoint;

    public Task<EmptyResponse> Execute(EmptyRequestProxy request)
        => Endpoint.Execute(request);
}