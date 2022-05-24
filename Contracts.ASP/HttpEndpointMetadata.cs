using System;

namespace Staticsoft.Contracts.ASP
{
    public interface HttpEndpointMetadata<TRequest, TResponse> : HttpEndpointMetadata { }

    public interface HttpEndpointMetadata
    {
        T GetAttribute<T>() where T : Attribute;
        string Path { get; }
        Type RequestType { get; }
        Type ResponseType { get; }
    }
}
