using Staticsoft.Contracts.Abstractions;

namespace Staticsoft.Contracts.ASP.Server;

public interface StreamableParametrizedHttpEndpointFactory
{
    StreamableParametrizedHttpEndpoint<RequestBody> Resolve<RequestBody>();
}
