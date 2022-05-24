using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.TestContract
{
    public class GroupWithSameEndpointName
    {
        public readonly HttpEndpoint<OtherThanSameNameRequest, OtherThanSameNameResponse> SameNameEndpoint;

        public GroupWithSameEndpointName(
            [Endpoint(HttpMethod.Post)]
            HttpEndpoint<OtherThanSameNameRequest, OtherThanSameNameResponse> sameNameEndpoint
        )
            => SameNameEndpoint = sameNameEndpoint;
    }
}
