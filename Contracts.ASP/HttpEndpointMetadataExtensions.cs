using System;

namespace Staticsoft.Contracts.ASP;

public static class HttpEndpointMetadataExtensions
{
    public static bool TryGetAttribute<T>(this HttpEndpointMetadata metadata, out T attribute) where T : Attribute
    {
        attribute = metadata.GetAttribute<T>();
        return attribute != null;
    }

    public static bool HasAttribute<T>(this HttpEndpointMetadata metadata) where T : Attribute
        => metadata.GetAttribute<T>() != null;
}
