using Staticsoft.Contracts.Abstractions;

namespace Staticsoft.Contracts.ASP.Server
{
    public interface HttpEndpointFactory
    {
        HttpEndpoint<TRequest, TResponse> Resolve<TRequest, TResponse>();
    }
}
