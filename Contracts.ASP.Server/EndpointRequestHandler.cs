using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Serialization.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server
{
    public class EndpointRequestHandler<TRequest, TResponse> : HttpRequestHandler<TRequest, TResponse>
    {
        readonly JsonSerializer Serializer;
        readonly HttpEndpoint<TRequest, TResponse> Endpoint;

        public EndpointRequestHandler(JsonSerializer serializer, HttpEndpoint<TRequest, TResponse> endpoint)
        {
            Serializer = serializer;
            Endpoint = endpoint;
        }

        public async Task Execute(HttpContext context)
        {
            var request = await ReadRequest(context);

            var response = await Endpoint.Execute(request);

            await WriteResponse(context, response);
        }

        async Task<TRequest> ReadRequest(HttpContext context)
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            var requestText = await reader.ReadToEndAsync();
            return Serializer.Deserialize<TRequest>(requestText);
        }

        async Task WriteResponse(HttpContext context, TResponse response)
        {
            var responseText = Serializer.Serialize(response);
            await context.Response.WriteAsync(responseText, Encoding.UTF8);
        }
    }

    public class EndpointRequestHandler : HttpRequestHandler
    {
        readonly HttpRequestHandlerFactory Factory;

        public EndpointRequestHandler(HttpRequestHandlerFactory factory)
            => Factory = factory;

        public Task Execute<TRequest, TResponse>(HttpContext context)
            => Factory.Create<TRequest, TResponse>().Execute(context);
    }
}
