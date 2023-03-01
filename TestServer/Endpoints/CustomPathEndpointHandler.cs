using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class CustomPathEndpointHandler : HttpEndpoint<EmptyRequest, RequestPathResponse>
    {
        readonly IHttpContextAccessor Accessor;

        public CustomPathEndpointHandler(IHttpContextAccessor accessor)
            => Accessor = accessor;

        public Task<RequestPathResponse> Execute(EmptyRequest request)
            => Task.FromResult(new RequestPathResponse() { RequestPath = Accessor.HttpContext.Request.Path });
    }
}