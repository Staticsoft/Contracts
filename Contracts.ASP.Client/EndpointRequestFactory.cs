using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.Contracts.ASP.Client
{
    public interface EndpointRequestFactory
    {
        HttpRequest Create(HttpEndpointMetadata metadata, string path, object body);
    }
}
