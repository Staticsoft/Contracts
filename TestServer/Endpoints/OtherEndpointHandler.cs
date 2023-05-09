using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer;

public class OtherEndpointHandler : HttpEndpoint<OtherThanSameNameRequest, OtherThanSameNameResponse>
{
    public Task<OtherThanSameNameResponse> Execute(OtherThanSameNameRequest request)
        => Task.FromResult(new OtherThanSameNameResponse { OtherOutput = request.OtherInput });
}