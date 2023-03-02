using System;

namespace Staticsoft.Contracts.ASP
{
    public interface HttpEndpointMetadata<RequestBody, ResponseBody> : HttpEndpointMetadata { }

    public interface HttpEndpointMetadata
    {
        T GetAttribute<T>() where T : Attribute;
        RequestMetadata Request { get; }
        ResponseMetadata Response { get; }
    }

    public class RequestMetadata
    {
        public RequestPattern Pattern { get; init; }
        public Type BodyType { get; init; }
    }

    public class RequestPattern
    {
        public string Value { get; init; }
        public PatternType Type { get; init; }
    }

    public class ResponseMetadata
    {
        public Type BodyType { get; init; }
    }
}
