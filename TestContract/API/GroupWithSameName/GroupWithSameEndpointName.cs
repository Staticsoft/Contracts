using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract;

public class GroupWithSameEndpointName(
    HttpEndpoint<OtherThanSameNameRequest, OtherThanSameNameResponse> sameNameEndpoint
)
{
    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<OtherThanSameNameRequest, OtherThanSameNameResponse> SameNameEndpoint { get; } = sameNameEndpoint;
}
