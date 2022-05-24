using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.Contracts.ASP.Client
{
    public class HttpEndpointRequestFactory : EndpointRequestFactory
    {
        readonly HttpRequestFactory Factory;

        public HttpEndpointRequestFactory(HttpRequestFactory factory)
            => Factory = factory;

        public HttpRequest Create(HttpEndpointMetadata metadata, object body)
            => Factory.Create(metadata.GetAttribute<EndpointAttribute>().Method, metadata.Path, body);
    }
}
