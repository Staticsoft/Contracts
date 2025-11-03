using Microsoft.AspNetCore.Http;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Serialization.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server;

public class StreamableEndpointRequestHandler : StreamableHttpRequestHandler
{
    readonly JsonSerializer Serializer;
    readonly StreamableHttpEndpointFactory Endpoint;
    readonly StreamableParametrizedHttpEndpointFactory ParametrizedEndpoint;

    public StreamableEndpointRequestHandler(
        JsonSerializer serializer,
        StreamableHttpEndpointFactory endpoint,
        StreamableParametrizedHttpEndpointFactory parametrizedEndpoint)
        => (Serializer, Endpoint, ParametrizedEndpoint)
        = (serializer, endpoint, parametrizedEndpoint);

    public async Task Execute<RequestBody>(HttpContext context, StreamableHttpEndpointMetadata metadata)
        where RequestBody : class, new()
    {
        var request = await ReadRequest<RequestBody>(context);

        context.Response.ContentType = "text/plain; charset=utf-8";
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers["X-Accel-Buffering"] = "no";

        await foreach (var chunk in ExecuteStreamingRequest(request, metadata, context))
        {
            await context.Response.WriteAsync(chunk, Encoding.UTF8);
            await context.Response.Body.FlushAsync();
        }
    }

    async IAsyncEnumerable<string> ExecuteStreamingRequest<RequestBody>(
        RequestBody request,
        StreamableHttpEndpointMetadata metadata,
        HttpContext context
    )
    {
        var stream = metadata.Request.Pattern.Type switch
        {
            PatternType.Static => ExecuteStaticStreamingRequest(request),
            PatternType.Parametrized => ExecuteParametrizedStreamingRequest(request, context, metadata),
            _ => throw new NotSupportedException($"{nameof(PatternType)} {metadata.Request.Pattern.Type} is not supported")
        };

        await foreach (var chunk in stream)
        {
            yield return chunk;
        }
    }

    IAsyncEnumerable<string> ExecuteStaticStreamingRequest<RequestBody>(RequestBody request)
        => Endpoint.Resolve<RequestBody>().Execute(request);

    IAsyncEnumerable<string> ExecuteParametrizedStreamingRequest<RequestBody>(
        RequestBody request,
        HttpContext context,
        StreamableHttpEndpointMetadata metadata
    )
        => ParametrizedEndpoint.Resolve<RequestBody>().Execute(GetParameter(context.Request.Path, metadata), request);

    static string GetParameter(string requestPath, StreamableHttpEndpointMetadata metadata)
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
}
