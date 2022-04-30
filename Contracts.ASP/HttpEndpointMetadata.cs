using Staticsoft.HttpCommunication.Abstractions;
using System;

namespace Staticsoft.Contracts.ASP
{
    public interface HttpEndpointMetadata<TRequest, TResponse> : HttpEndpointMetadata { }

    public interface HttpEndpointMetadata
    {
        HttpMethod Method { get; }
        string Path { get; }
        Type RequestType { get; }
        Type ResponseType { get; }
    }
}
