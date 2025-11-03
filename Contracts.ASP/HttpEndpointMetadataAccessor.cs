using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Staticsoft.Contracts.ASP;

public static class HttpEndpointMetadataAccessor
{
    static readonly Type HttpEndpointType = typeof(HttpEndpoint<,>);
    static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);
    static readonly Type StreamableHttpEndpointType = typeof(StreamableHttpEndpoint<>);
    static readonly Type StreamableParametrizedHttpEndpointType = typeof(StreamableParametrizedHttpEndpoint<>);

    #region Metadata
    public static IServiceCollection AddMetadata(
        IServiceCollection services,
        IEnumerable<HttpEndpointMetadata> metadata
    )
    {
        AssertNoDuplicates(metadata);

        return metadata.Aggregate(
            services,
            (services, metadata) => services.AddSingleton(MakeHttpEndpointMetadata(metadata), metadata)
        );
    }

    public static IEnumerable<HttpEndpointMetadata> GetMetadata(Type type)
        => GetMetadata(type, string.Empty);

    static IEnumerable<HttpEndpointMetadata> GetMetadata(Type type, string basePattern)
        => type.GetProperties().SelectMany(property => GetMetadata(property, basePattern));

    static IEnumerable<HttpEndpointMetadata> GetMetadata(PropertyInfo property, string basePattern)
        => property.PropertyType.IsGenericTypeOf(HttpEndpointType)
        || property.PropertyType.IsGenericTypeOf(ParametrizedHttpEndpointType)
        ? [CreateMetadata(property, basePattern)]
        : GetMetadata(property.PropertyType, $"{basePattern}/{property.Name}");

    static HttpEndpointMetadata CreateMetadata(PropertyInfo property, string basePattern)
    {
        var (requestType, responseType) = GetRequestResponseTypes(property.PropertyType);
        return CreateMetadata(property, basePattern, requestType, responseType);
    }

    static (Type, Type) GetRequestResponseTypes(Type type)
        => GetRequestResponseTypes(type.GetGenericArguments());

    static (Type, Type) GetRequestResponseTypes(Type[] types)
        => (types.First(), types.Last());

    static HttpEndpointMetadata CreateMetadata(PropertyInfo property, string basePattern, Type requestType, Type responseType)
        => typeof(ReflectionHttpEndpointMetadata<,>)
            .MakeGenericType(requestType, responseType)
            .GetConstructor()
            .Invoke([property, basePattern]) as HttpEndpointMetadata;

    static Type MakeHttpEndpointMetadata(HttpEndpointMetadata metadata)
        => typeof(HttpEndpointMetadata<,>).MakeGenericType(metadata.Request.BodyType, metadata.Response.BodyType);
    #endregion

    #region StreamableMetadata
    public static IServiceCollection AddStreamableMetadata(
        IServiceCollection services,
        IEnumerable<StreamableHttpEndpointMetadata> streamableMetadata
    )
    {
        AssertNoDuplicates(streamableMetadata);

        return streamableMetadata.Aggregate(
            services,
            (services, metadata) => services.AddSingleton(MakeStreamableHttpEndpointMetadata(metadata), metadata)
        );
    }

    public static IEnumerable<StreamableHttpEndpointMetadata> GetStreamableMetadata(Type type)
        => GetStreamableMetadata(type, string.Empty);

    static IEnumerable<StreamableHttpEndpointMetadata> GetStreamableMetadata(Type type, string basePattern)
        => type.GetProperties().SelectMany(property => GetStreamableMetadata(property, basePattern));

    static IEnumerable<StreamableHttpEndpointMetadata> GetStreamableMetadata(PropertyInfo property, string basePattern)
        => property.PropertyType.IsGenericTypeOf(StreamableHttpEndpointType)
        || property.PropertyType.IsGenericTypeOf(StreamableParametrizedHttpEndpointType)
        ? [CreateStreamableMetadata(property, basePattern)]
        : GetStreamableMetadata(property.PropertyType, $"{basePattern}/{property.Name}");

    static StreamableHttpEndpointMetadata CreateStreamableMetadata(PropertyInfo property, string basePattern)
    {
        var requestType = property.PropertyType.GetGenericArguments().Single();
        return CreateStreamableMetadata(property, basePattern, requestType);
    }

    static StreamableHttpEndpointMetadata CreateStreamableMetadata(PropertyInfo property, string basePattern, Type requestType)
        => typeof(ReflectionStreamableHttpEndpointMetadata<>)
            .MakeGenericType(requestType)
            .GetConstructor()
            .Invoke([property, basePattern]) as StreamableHttpEndpointMetadata;

    static Type MakeStreamableHttpEndpointMetadata(StreamableHttpEndpointMetadata metadata)
        => typeof(StreamableHttpEndpointMetadata<>).MakeGenericType(metadata.Request.BodyType);
    #endregion

    static void AssertNoDuplicates<T>(IEnumerable<T> types)
    {
        var duplicates = GetDuplicates(types);
        if (duplicates.Any())
        {
            throw new Exception($"Duplicate metadata: {string.Join(", ", duplicates)}");
        }
    }

    static IEnumerable<T> GetDuplicates<T>(IEnumerable<T> types)
    {
        var set = new HashSet<T>();
        foreach (var type in types)
        {
            if (!set.Add(type))
            {
                yield return type;
            }
        }
    }
}
