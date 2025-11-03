using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract;

public class NestedGroup(
    HttpEndpoint<EmptyRequest, NestedRequestPathResponse> nestedEndpoint
)
{
    [Endpoint(HttpMethod.Get)]
    public HttpEndpoint<EmptyRequest, NestedRequestPathResponse> NestedEndpoint { get; } = nestedEndpoint;
}
