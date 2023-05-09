using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract;

public class TestGroup
{
    public TestGroup(
        HttpEndpoint<TestRequest, TestResponse> testEndpoint,
        HttpEndpoint<EmptyRequest, EmptyResponse> emptyEndpoint,
        HttpEndpoint<EmptyRequestProxy, EmptyResponse> emptyEndpointProxy,
        HttpEndpoint<SameNameRequest, SameNameResponse> sameNameEndpoint,
        ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> emptyParametrizedEndpoint,
        HttpEndpoint<EmptyRequest, CustomRequestPathResponse> customPathEndpoint,
        NestedGroup nested,
        HttpEndpoint<EmptyRequest, CustomStatusCodeResponse> customStatusCodeEndpoint
    )
    {
        TestEndpoint = testEndpoint;
        EmptyEndpoint = emptyEndpoint;
        EmptyEndpointProxy = emptyEndpointProxy;
        SameNameEndpoint = sameNameEndpoint;
        EmptyParametrizedEndpoint = emptyParametrizedEndpoint;
        CustomPathEndpoint = customPathEndpoint;
        Nested = nested;
        CustomStatusCodeEndpoint = customStatusCodeEndpoint;
    }

    [Endpoint(HttpMethod.Post)]
    [AuthenticateRequest]
    public HttpEndpoint<TestRequest, TestResponse> TestEndpoint { get; }

    [Endpoint(HttpMethod.Get)]
    public HttpEndpoint<EmptyRequest, EmptyResponse> EmptyEndpoint { get; }

    [Endpoint(HttpMethod.Get)]
    public HttpEndpoint<EmptyRequestProxy, EmptyResponse> EmptyEndpointProxy { get; }

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<SameNameRequest, SameNameResponse> SameNameEndpoint { get; }

    [Endpoint(HttpMethod.Get)]
    public ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> EmptyParametrizedEndpoint { get; }

    [Endpoint(HttpMethod.Get, pattern: "custom")]
    public HttpEndpoint<EmptyRequest, CustomRequestPathResponse> CustomPathEndpoint { get; }

    public NestedGroup Nested { get; }

    [Endpoint(HttpMethod.Get)]
    [EndpointBehavior(statusCode: 234)]
    public HttpEndpoint<EmptyRequest, CustomStatusCodeResponse> CustomStatusCodeEndpoint { get; }
}
