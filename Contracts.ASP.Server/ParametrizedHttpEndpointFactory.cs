using Staticsoft.Contracts.Abstractions;

namespace Staticsoft.Contracts.ASP.Server;

public interface ParametrizedHttpEndpointFactory
{
    ParametrizedHttpEndpoint<RequestBody, ResponseBody> Resolve<RequestBody, ResponseBody>();
}
