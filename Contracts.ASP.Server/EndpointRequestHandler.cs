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

        public async Task Execute<RequestBody, ResponseBody>(HttpContext context, HttpEndpointMetadata metadata)
        {
            var request = await ReadRequest<RequestBody>(context);

            var response = await Endpoint.Resolve<RequestBody, ResponseBody>().Execute(request);

            await WriteResponse(context, response);
        }

        async Task<RequestBody> ReadRequest<RequestBody>(HttpContext context)
        {
            if (typeof(RequestBody) == typeof(EmptyRequest)) return (RequestBody)(object)new EmptyRequest();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            var requestText = await reader.ReadToEndAsync();
            return Serializer.Deserialize<RequestBody>(requestText);
        }

        async Task WriteResponse<ResponseBody>(HttpContext context, ResponseBody response)
        {
            var responseText = Serializer.Serialize(response);
            await context.Response.WriteAsync(responseText, Encoding.UTF8);
        }
    }
}
