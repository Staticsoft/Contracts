using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract
{
    public class TestAPIGroup
    {
        public readonly HttpEndpoint<TestRequest, TestResponse> TestEndpoint;
        public readonly HttpEndpoint<EmptyRequest, EmptyResponse> EmptyEndpoint;
        public readonly HttpEndpoint<SameNameRequest, SameNameResponse> SameNameEndpoint;

        public TestAPIGroup(
            [Endpoint(HttpMethod.Post)]
            [AuthenticateRequest]
            HttpEndpoint<TestRequest, TestResponse> testEndpoint,

            [Endpoint(HttpMethod.Get)]
            HttpEndpoint<EmptyRequest, EmptyResponse> emptyEndpoint,

            [Endpoint(HttpMethod.Post)]
            HttpEndpoint<SameNameRequest, SameNameResponse> sameNameEndpoint
        )
        {
            TestEndpoint = testEndpoint;
            EmptyEndpoint = emptyEndpoint;
            SameNameEndpoint = sameNameEndpoint;
        }
    }
}
