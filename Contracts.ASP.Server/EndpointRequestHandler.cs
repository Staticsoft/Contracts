using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Serialization.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server
{
    public class EndpointRequestHandler : HttpRequestHandler
    {
        readonly JsonSerializer Serializer;
        readonly HttpEndpointFactory Endpoint;

        public EndpointRequestHandler(JsonSerializer serializer, HttpEndpointFactory factory)
            => (Serializer, Endpoint)
            = (serializer, factory);

        public async Task Execute<TRequest, TResponse>(HttpContext context)
        {
            var request = await ReadRequest<TRequest>(context);

            var response = await Endpoint.Resolve<TRequest, TResponse>().Execute(request);

            await WriteResponse(context, response);
        }

        async Task<TRequest> ReadRequest<TRequest>(HttpContext context)
        {
            if (typeof(TRequest) == typeof(EmptyRequest)) return (TRequest)(object)new EmptyRequest();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            var requestText = await reader.ReadToEndAsync();
            return Serializer.Deserialize<TRequest>(requestText);
        }

        async Task WriteResponse<TResponse>(HttpContext context, TResponse response)
        {
            var responseText = Serializer.Serialize(response);
            await context.Response.WriteAsync(responseText, Encoding.UTF8);
        }
    }
}
