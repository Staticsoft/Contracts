using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class CustomPathEndpointHandler : HttpEndpoint<EmptyRequest, CustomRequestPathResponse>
    {
        readonly IHttpContextAccessor Accessor;

        public CustomPathEndpointHandler(IHttpContextAccessor accessor)
            => Accessor = accessor;

        public Task<CustomRequestPathResponse> Execute(EmptyRequest request)
            => Task.FromResult(new CustomRequestPathResponse() { RequestPath = Accessor.HttpContext.Request.Path });
    }
}