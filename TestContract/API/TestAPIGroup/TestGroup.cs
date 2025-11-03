using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract;

public class TestGroup(
    HttpEndpoint<TestRequest, TestResponse> testEndpoint,
    HttpEndpoint<EmptyRequest, EmptyResponse> emptyEndpoint,
    HttpEndpoint<EmptyRequestProxy, EmptyResponse> emptyEndpointProxy,
    HttpEndpoint<SameNameRequest, SameNameResponse> sameNameEndpoint,
    ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> emptyParametrizedEndpoint,
    HttpEndpoint<EmptyRequest, CustomRequestPathResponse> customPathEndpoint,
    NestedGroup nested,
    HttpEndpoint<EmptyRequest, CustomStatusCodeResponse> customStatusCodeEndpoint,
    StreamableHttpEndpoint<EmptyRequest> streamingEndpoint
)
{
    [Endpoint(HttpMethod.Post)]
    [AuthenticateRequest]
    public HttpEndpoint<TestRequest, TestResponse> TestEndpoint { get; } = testEndpoint;

    [Endpoint(HttpMethod.Get)]
    public HttpEndpoint<EmptyRequest, EmptyResponse> EmptyEndpoint { get; } = emptyEndpoint;

    [Endpoint(HttpMethod.Get)]
    public HttpEndpoint<EmptyRequestProxy, EmptyResponse> EmptyEndpointProxy { get; } = emptyEndpointProxy;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<SameNameRequest, SameNameResponse> SameNameEndpoint { get; } = sameNameEndpoint;

    [Endpoint(HttpMethod.Get)]
    public ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> EmptyParametrizedEndpoint { get; } = emptyParametrizedEndpoint;

    [Endpoint(HttpMethod.Get, pattern: "custom")]
    public HttpEndpoint<EmptyRequest, CustomRequestPathResponse> CustomPathEndpoint { get; } = customPathEndpoint;

    public NestedGroup Nested { get; } = nested;

    [Endpoint(HttpMethod.Get)]
    [EndpointBehavior(statusCode: 234)]
    public HttpEndpoint<EmptyRequest, CustomStatusCodeResponse> CustomStatusCodeEndpoint { get; } = customStatusCodeEndpoint;

    [Endpoint(HttpMethod.Get)]
    public StreamableHttpEndpoint<EmptyRequest> StreamableEndpoint { get; } = streamingEndpoint;
}
