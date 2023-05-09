using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer;

public class CustomStatusCodeEndpointHandler : HttpEndpoint<EmptyRequest, CustomStatusCodeResponse>
{
    public Task<CustomStatusCodeResponse> Execute(EmptyRequest request)
        => Task.FromResult(new CustomStatusCodeResponse());
}