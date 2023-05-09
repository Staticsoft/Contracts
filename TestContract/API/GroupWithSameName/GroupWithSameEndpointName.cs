using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract;

public class GroupWithSameEndpointName
{
    public GroupWithSameEndpointName(
        HttpEndpoint<OtherThanSameNameRequest, OtherThanSameNameResponse> sameNameEndpoint
    )
        => SameNameEndpoint = sameNameEndpoint;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<OtherThanSameNameRequest, OtherThanSameNameResponse> SameNameEndpoint { get; }
}
