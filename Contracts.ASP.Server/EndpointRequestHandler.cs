using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Serialization.Abstractions;
using System;
using System.IO;
using System.Linq;
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
            where RequestBody : class, new()
            where ResponseBody : class, new()
        {
            var request = await ReadRequest<RequestBody>(context);

            var response = await ExecuteRequest<RequestBody, ResponseBody>(request, metadata, context);

            await WriteResponse(context, response, metadata);
        }

        Task<ResponseBody> ExecuteRequest<RequestBody, ResponseBody>(
            RequestBody request,
            HttpEndpointMetadata metadata,
            HttpContext context
        ) => metadata.Request.Pattern.Type switch
        {
            PatternType.Static => ExecuteStaticRequest<RequestBody, ResponseBody>(request),
            PatternType.Parametrized => ExecuteParametrizedRequest<RequestBody, ResponseBody>(request, context, metadata),
            _ => throw new NotSupportedException($"{nameof(PatternType)} {metadata.Request.Pattern.Type} is not supported")
        };

        Task<ResponseBody> ExecuteStaticRequest<RequestBody, ResponseBody>(RequestBody request)
            => Endpoint.Resolve<RequestBody, ResponseBody>().Execute(request);

        Task<ResponseBody> ExecuteParametrizedRequest<RequestBody, ResponseBody>(RequestBody request, HttpContext context, HttpEndpointMetadata metadata)
            => ParametrizedEndpoint.Resolve<RequestBody, ResponseBody>().Execute(GetParameter(context.Request.Path, metadata), request);

        static string GetParameter(string requestPath, HttpEndpointMetadata metadata)
        {
            var sections = metadata.Request.Pattern.Value.Split('/').Select((section, index) => new { Value = section, Index = index });
            var parameterSection = sections.Single(section => section.Value == "{parameter}");
            return requestPath.Split('/')[parameterSection.Index];
        }

        async Task<RequestBody> ReadRequest<RequestBody>(HttpContext context)
            where RequestBody : class, new()
        {
            if (typeof(RequestBody).IsAssignableTo(typeof(EmptyRequest))) return new();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            var requestText = await reader.ReadToEndAsync();
            return Serializer.Deserialize<RequestBody>(requestText);
        }

        async Task WriteResponse<ResponseBody>(HttpContext context, ResponseBody response, HttpEndpointMetadata metadata)
        {
            if (metadata.TryGetAttribute<EndpointBehaviorAttribute>(out var behavior))
            {
                context.Response.StatusCode = behavior.StatusCode;
            }
            var responseText = Serializer.Serialize(response);
            await context.Response.WriteAsync(responseText, Encoding.UTF8);
        }
    }
}
