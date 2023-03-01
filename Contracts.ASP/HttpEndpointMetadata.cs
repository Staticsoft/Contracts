using System;

namespace Staticsoft.Contracts.ASP
{
    public interface HttpEndpointMetadata<RequestBody, ResponseBody> : HttpEndpointMetadata { }

    public interface HttpEndpointMetadata
    {
        T GetAttribute<T>() where T : Attribute;
        string Pattern { get; }
        Type RequestBodyType { get; }
        Type ResponseBodyType { get; }
        RequestType RequestType { get; }
    }
}
