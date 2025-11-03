using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP;

public class ReflectionStreamableHttpEndpointMetadata<RequestBody>(
    PropertyInfo property,
    string basePattern
) : StreamableHttpEndpointMetadata<RequestBody>
{
    static readonly Type StreamableParametrizedHttpEndpointType = typeof(StreamableParametrizedHttpEndpoint<>);

    readonly PropertyInfo Property = property;

    public RequestMetadata Request { get; } = ReflectionMetadata.BuildRequestMetadata<RequestBody>(property, basePattern, StreamableParametrizedHttpEndpointType);

    public T GetAttribute<T>() where T : Attribute
        => Property.GetCustomAttribute<T>();

    public override bool Equals(object obj) => obj switch
    {
        ReflectionStreamableHttpEndpointMetadata<RequestBody> metadata => Equals(metadata),
        _ => false
    };

    bool Equals(ReflectionStreamableHttpEndpointMetadata<RequestBody> metadata)
        => metadata.Request.BodyType == Request.BodyType;

    public override int GetHashCode()
        => Request.BodyType.GetHashCode();

    public override string ToString()
        => $"{nameof(StreamableHttpEndpointMetadata)}<{Request.BodyType.Name}>";
}
