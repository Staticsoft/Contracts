using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class EmptyEndpointHandlerProxy : HttpEndpoint<EmptyRequestProxy, EmptyResponse>
    {
        readonly HttpEndpoint<EmptyRequest, EmptyResponse> Endpoint;

        public EmptyEndpointHandlerProxy(HttpEndpoint<EmptyRequest, EmptyResponse> endpoint)
            => Endpoint = endpoint;

        public Task<EmptyResponse> Execute(EmptyRequestProxy request)
            => Endpoint.Execute(request);
    }
}