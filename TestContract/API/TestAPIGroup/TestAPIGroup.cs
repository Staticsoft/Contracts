using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract
{
    public class TestAPIGroup
    {
        public readonly HttpEndpoint<TestRequest, TestResponse> TestEndpoint;
        public readonly HttpEndpoint<EmptyRequest, EmptyResponse> EmptyEndpoint;
        public readonly HttpEndpoint<SameNameRequest, SameNameResponse> SameNameEndpoint;
        public readonly ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> EmptyParametrizedEndpoint;
        public readonly HttpEndpoint<EmptyRequest, RequestPathResponse> CustomPathEndpoint;

        public TestAPIGroup(
            [Endpoint(HttpMethod.Post)]
            [AuthenticateRequest]
            HttpEndpoint<TestRequest, TestResponse> testEndpoint,

            [Endpoint(HttpMethod.Get)]
            HttpEndpoint<EmptyRequest, EmptyResponse> emptyEndpoint,

            [Endpoint(HttpMethod.Post)]
            HttpEndpoint<SameNameRequest, SameNameResponse> sameNameEndpoint,

            [Endpoint(HttpMethod.Get)]
            ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse> emptyParametrizedEndpoint,

            [Endpoint(HttpMethod.Get, "custom")]
            HttpEndpoint<EmptyRequest, RequestPathResponse> customPathEndpoint
        )
        {
            TestEndpoint = testEndpoint;
            EmptyEndpoint = emptyEndpoint;
            SameNameEndpoint = sameNameEndpoint;
            EmptyParametrizedEndpoint = emptyParametrizedEndpoint;
            CustomPathEndpoint = customPathEndpoint;
        }
    }
}
