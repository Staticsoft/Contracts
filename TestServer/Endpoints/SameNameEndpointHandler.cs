using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class SameNameEndpointHandler : HttpEndpoint<SameNameRequest, SameNameResponse>
    {
        public Task<SameNameResponse> Execute(SameNameRequest request)
            => Task.FromResult(new SameNameResponse { TestOutput = request.TestInput });
    }
}