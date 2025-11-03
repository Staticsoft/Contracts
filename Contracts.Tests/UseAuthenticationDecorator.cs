using Staticsoft.Contracts.ASP;
using Staticsoft.Contracts.ASP.Client;
using Staticsoft.HttpCommunication.Abstractions;
using Staticsoft.TestContract;

namespace Staticsoft.Contracts.Tests;

public class UseAuthenticationDecorator(
    EndpointRequestFactory factory,
    Authentication authentication
) : EndpointRequestFactory
{
    readonly EndpointRequestFactory Factory = factory;
    readonly Authentication Authentication = authentication;

    public HttpRequest Create(HttpEndpointMetadata metadata, string path, object body)
        => Create(metadata, Factory.Create(metadata, path, body));

    HttpRequest Create(HttpEndpointMetadata metadata, HttpRequest request)
        => metadata.HasAttribute<AuthenticateRequestAttribute>()
        ? Decorate(request)
        : request;

    HttpRequest Decorate(HttpRequest request)
        => request.WithHeader(Authentication.Get().Name, Authentication.Get().Value);
}
