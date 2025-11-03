using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP;

public class ReflectionHttpEndpointMetadata<RequestBody, ResponseBody>(
    PropertyInfo property,
    string basePattern
) : HttpEndpointMetadata<RequestBody, ResponseBody>
{
    static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

    readonly PropertyInfo Property = property;

    public RequestMetadata Request { get; } = ReflectionMetadata.BuildRequestMetadata<RequestBody>(property, basePattern, ParametrizedHttpEndpointType);
    public ResponseMetadata Response { get; } = new()
    {
        BodyType = typeof(ResponseBody)
    };

    public T GetAttribute<T>() where T : Attribute
        => Property.GetCustomAttribute<T>();

    public override bool Equals(object obj) => obj switch
    {
        ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> metadata => Equals(metadata),
        _ => false
    };

    bool Equals(ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> metadata)
        => metadata.Request.BodyType == Request.BodyType
        && metadata.Response.BodyType == Response.BodyType;

    public override int GetHashCode()
        => HashCode.Combine(Request.BodyType, Response.BodyType);

    public override string ToString()
        => $"{nameof(HttpEndpointMetadata)}<{Request.BodyType.Name}, {Response.BodyType.Name}>";
}
