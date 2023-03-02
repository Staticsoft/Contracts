using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract
{
    public class NestedGroup
    {
        public NestedGroup(HttpEndpoint<EmptyRequest, NestedRequestPathResponse> nestedEndpoint)
            => NestedEndpoint = nestedEndpoint;

        [Endpoint(HttpMethod.Get)]
        public HttpEndpoint<EmptyRequest, NestedRequestPathResponse> NestedEndpoint { get; }

    }
}
