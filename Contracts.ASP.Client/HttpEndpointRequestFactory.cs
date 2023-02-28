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
            => Create(metadata.GetAttribute<EndpointAttribute>().Method, metadata.Path, body);

        HttpRequest Create(HttpMethod method, string path, object body)
            => method == HttpMethod.Get
            ? Factory.Create(method, path)
            : Factory.Create(method, path, body);
    }
}
