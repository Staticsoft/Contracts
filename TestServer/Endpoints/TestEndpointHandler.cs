using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer;

public class TestEndpointHandler : HttpEndpoint<TestRequest, TestResponse>
{
    public Task<TestResponse> Execute(TestRequest request)
        => Task.FromResult(new TestResponse { TestOutput = request.TestInput });
}