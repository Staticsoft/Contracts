using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.Contracts.ASP.Client
{
    public class HttpEndpointRequestFactory : EndpointRequestFactory
    {
        readonly HttpRequestFactory Factory;

        public HttpEndpointRequestFactory(HttpRequestFactory factory)
            => Factory = factory;

        public HttpRequest Create(HttpEndpointMetadata metadata, string path, object body)
            => Create(metadata.GetAttribute<EndpointAttribute>().Method, path, body);

        HttpRequest Create(HttpMethod method, string path, object body) => body switch
        {
            EmptyRequest => Factory.Create(method, path),
            _ => Factory.Create(method, path, body)
        };
    }
}
