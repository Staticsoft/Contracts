using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract
{
    public class TestAPIGroup
    {
        public readonly HttpEndpoint<TestRequest, TestResponse> TestEndpoint;
        public readonly HttpEndpoint<EmptyRequest, EmptyResponse> EmptyEndpoint;

        public TestAPIGroup(
            [Endpoint(HttpMethod.Post)] HttpEndpoint<TestRequest, TestResponse> testEndpoint,
            [Endpoint(HttpMethod.Get)] HttpEndpoint<EmptyRequest, EmptyResponse> emptyEndpoint
        )
        {
            TestEndpoint = testEndpoint;
            EmptyEndpoint = emptyEndpoint;
        }
    }
}
