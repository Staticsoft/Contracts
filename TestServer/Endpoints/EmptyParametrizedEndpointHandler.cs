using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer;

public class EmptyParametrizedEndpointHandler : ParametrizedHttpEndpoint<EmptyRequest, RequestParameterResponse>
{
    public Task<RequestParameterResponse> Execute(string parameter, EmptyRequest request)
        => Task.FromResult(new RequestParameterResponse() { Parameter = parameter });
}