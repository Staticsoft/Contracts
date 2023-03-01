using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Serialization.Abstractions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server
{
    public class EndpointRequestHandler : HttpRequestHandler
    {
        readonly JsonSerializer Serializer;
        readonly HttpEndpointFactory Endpoint;
        readonly ParametrizedHttpEndpointFactory ParametrizedEndpoint;

        public EndpointRequestHandler(JsonSerializer serializer, HttpEndpointFactory factory, ParametrizedHttpEndpointFactory parametrizedEndpoint)
            => (Serializer, Endpoint, ParametrizedEndpoint)
            = (serializer, factory, parametrizedEndpoint);

        public async Task Execute<RequestBody, ResponseBody>(HttpContext context, HttpEndpointMetadata metadata)
        {
            var request = await ReadRequest<RequestBody>(context);

            var response = await ExecuteRequest<RequestBody, ResponseBody>(request, metadata, context);

            await WriteResponse(context, response);
        }

        Task<ResponseBody> ExecuteRequest<RequestBody, ResponseBody>(
            RequestBody request,
            HttpEndpointMetadata metadata,
            HttpContext context
        ) => metadata.RequestType switch
        {
            RequestType.Static => ExecuteStaticRequest<RequestBody, ResponseBody>(request),
            RequestType.Parametrized => ExecuteParametrizedRequest<RequestBody, ResponseBody>(request, context),
            _ => throw new NotSupportedException($"{nameof(RequestType)} {metadata.RequestType} is not supported")
        };

        Task<ResponseBody> ExecuteStaticRequest<RequestBody, ResponseBody>(RequestBody request)
            => Endpoint.Resolve<RequestBody, ResponseBody>().Execute(request);

        Task<ResponseBody> ExecuteParametrizedRequest<RequestBody, ResponseBody>(RequestBody request, HttpContext context)
            => ParametrizedEndpoint.Resolve<RequestBody, ResponseBody>().Execute(GetParameter(context.Request.Path), request);

        static string GetParameter(string requestPath)
            => requestPath[(requestPath.LastIndexOf('/') + 1)..];

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
