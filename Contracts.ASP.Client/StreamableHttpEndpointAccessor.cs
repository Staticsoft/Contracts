using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Staticsoft.Contracts.ASP.Client;

public class StreamableHttpEndpointAccessor<RequestBody>(
    StreamableHttpEndpointMetadata<RequestBody> metadata,
    EndpointRequestFactory factory,
    HttpClient client
) : StreamableHttpEndpoint<RequestBody>,
    StreamableParametrizedHttpEndpoint<RequestBody>
{
    readonly StreamableHttpEndpointMetadata Metadata = metadata;
    readonly EndpointRequestFactory Factory = factory;
    readonly HttpClient Client = client;

    public IAsyncEnumerable<string> Execute(RequestBody body)
        => ExecuteStreamingRequest(Factory.CreateStreamable(Metadata, Metadata.Request.Pattern.Value, body));

    public IAsyncEnumerable<string> Execute(string parameter, RequestBody body)
        => ExecuteStreamingRequest(Factory.CreateStreamable(Metadata, Metadata.Request.Pattern.Value.Replace("{parameter}", parameter), body));

    async IAsyncEnumerable<string> ExecuteStreamingRequest(HttpRequest request)
    {
        using var response = await Client.SendAsync(CreateMessage(request), HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var buffer = new char[1024];
        var bytesRead = 0;

        while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            yield return new string(buffer, 0, bytesRead);
        }
    }

    static HttpRequestMessage CreateMessage(HttpRequest request)
    {
        var message = new HttpRequestMessage()
        {
            Method = GetHttpMethod(request.Method),
            RequestUri = new Uri(request.Path, UriKind.RelativeOrAbsolute)
        };
        SetContent(message, request.Body);
        AddHeaders(message.Headers, request.Headers);
        return message;
    }

    static System.Net.Http.HttpMethod GetHttpMethod(HttpCommunication.Abstractions.HttpMethod method) => method switch
    {
        HttpCommunication.Abstractions.HttpMethod.Get => System.Net.Http.HttpMethod.Get,
        HttpCommunication.Abstractions.HttpMethod.Post => System.Net.Http.HttpMethod.Post,
        HttpCommunication.Abstractions.HttpMethod.Put => System.Net.Http.HttpMethod.Put,
        HttpCommunication.Abstractions.HttpMethod.Delete => System.Net.Http.HttpMethod.Delete,
        _ => throw new NotSupportedException($"HTTP method {method} is not supported")
    };

    static void SetContent(HttpRequestMessage request, HttpBody body)
    {
        if (body.Value.Length == 0) return;

        var content = new ByteArrayContent(body.Value);
        content.Headers.ContentType = new MediaTypeHeaderValue(body.ContentType);

        request.Content = content;
    }

    static void AddHeaders(HttpRequestHeaders httpHeaders, IReadOnlyDictionary<string, string> requestHeaders)
    {
        foreach (var (name, value) in requestHeaders)
        {
            httpHeaders.Add(name, value);
        }
    }
}
