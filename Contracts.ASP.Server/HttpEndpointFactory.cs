using Staticsoft.Contracts.Abstractions;

namespace Staticsoft.Contracts.ASP.Server
{
    public interface HttpEndpointFactory
    {
        HttpEndpoint<RequestBody, ResponseBody> Resolve<RequestBody, ResponseBody>();
    }
}
