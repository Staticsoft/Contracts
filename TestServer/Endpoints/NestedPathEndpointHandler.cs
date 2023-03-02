using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class NestedPathEndpointHandler : HttpEndpoint<EmptyRequest, NestedRequestPathResponse>
    {
        readonly IHttpContextAccessor Accessor;

        public NestedPathEndpointHandler(IHttpContextAccessor accessor)
            => Accessor = accessor;

        public Task<NestedRequestPathResponse> Execute(EmptyRequest request)
            => Task.FromResult(new NestedRequestPathResponse() { RequestPath = Accessor.HttpContext.Request.Path });
    }
}