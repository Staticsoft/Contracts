using Staticsoft.Contracts.Abstractions;

namespace Staticsoft.Contracts.ASP.Server;

public interface StreamableHttpEndpointFactory
{
    StreamableHttpEndpoint<RequestBody> Resolve<RequestBody>();
}
