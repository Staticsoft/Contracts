using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract
{
    public class TestGroup
    {
        public TestGroup(
            HttpEndpoint<TestRequest, TestResponse> testEndpoint,
            HttpEndpoint<EmptyRequest, EmptyResponse> emptyEndpoint,
            HttpEndpoint<SameNameRequest, SameNameResponse> sameNameEndpoint,
            ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> emptyParametrizedEndpoint,
            HttpEndpoint<EmptyRequest, RequestPathResponse> customPathEndpoint
        )
        {
            TestEndpoint = testEndpoint;
            EmptyEndpoint = emptyEndpoint;
            SameNameEndpoint = sameNameEndpoint;
            EmptyParametrizedEndpoint = emptyParametrizedEndpoint;
            CustomPathEndpoint = customPathEndpoint;
        }

        [Endpoint(HttpMethod.Post)]
        [AuthenticateRequest]
        public HttpEndpoint<TestRequest, TestResponse> TestEndpoint { get; }

        [Endpoint(HttpMethod.Get)]
        public HttpEndpoint<EmptyRequest, EmptyResponse> EmptyEndpoint { get; }

        [Endpoint(HttpMethod.Post)]
        public HttpEndpoint<SameNameRequest, SameNameResponse> SameNameEndpoint { get; }

        [Endpoint(HttpMethod.Get)]
        public ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> EmptyParametrizedEndpoint { get; }

        [Endpoint(HttpMethod.Get, "custom")]
        public HttpEndpoint<EmptyRequest, RequestPathResponse> CustomPathEndpoint { get; }
    }
}
