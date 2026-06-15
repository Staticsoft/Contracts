using System;
using System.Linq;

namespace Staticsoft.Contracts.ASP;

public class GeneratedHttpEndpointMetadata<RequestBody, ResponseBody>(
    string pattern,
    PatternType patternType,
    Attribute[] attributes
) : HttpEndpointMetadata<RequestBody, ResponseBody>
{
    readonly Attribute[] Attributes = attributes;

    public RequestMetadata Request { get; } = new()
    {
        BodyType = typeof(RequestBody),
        Pattern = new() { Value = pattern, Type = patternType }
    };

    public ResponseMetadata Response { get; } = new()
    {
        BodyType = typeof(ResponseBody)
    };

    public T GetAttribute<T>() where T : Attribute
        => Attributes.OfType<T>().FirstOrDefault();

    public override bool Equals(object obj)
        => obj is GeneratedHttpEndpointMetadata<RequestBody, ResponseBody>;

    public override int GetHashCode()
        => HashCode.Combine(typeof(RequestBody), typeof(ResponseBody));

    public override string ToString()
        => $"HttpEndpointMetadata<{typeof(RequestBody).Name}, {typeof(ResponseBody).Name}>";
}
