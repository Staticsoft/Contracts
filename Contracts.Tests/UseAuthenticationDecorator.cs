using Staticsoft.Contracts.ASP;
using Staticsoft.Contracts.ASP.Client;
using Staticsoft.HttpCommunication.Abstractions;
using Staticsoft.TestContract;

namespace Staticsoft.Contracts.Tests
{
    public class UseAuthenticationDecorator : EndpointRequestFactory
    {
        readonly EndpointRequestFactory Factory;
        readonly Authentication Authentication;

        public UseAuthenticationDecorator(EndpointRequestFactory factory, Authentication authentication)
        {
            Factory = factory;
            Authentication = authentication;
        }

        public HttpRequest Create(HttpEndpointMetadata metadata, object body)
            => Create(metadata, Factory.Create(metadata, body));

        HttpRequest Create(HttpEndpointMetadata metadata, HttpRequest request)
            => metadata.HasAttribute<AuthenticateRequestAttribute>()
            ? Decorate(request)
            : request;

        HttpRequest Decorate(HttpRequest request)
            => request.WithHeader(Authentication.Get().Name, Authentication.Get().Value);
    }
}
