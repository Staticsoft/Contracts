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
        || property.PropertyType.IsGenericTypeOf(StreamableHttpEndpointType)
        || property.PropertyType.IsGenericTypeOf(StreamableParametrizedHttpEndpointType)
        ? [CreateMetadata(property, basePattern)]
        : GetMetadata(property.PropertyType, $"{basePattern}/{property.Name}");

    static HttpEndpointMetadata CreateMetadata(PropertyInfo property, string basePattern)
    {
        var types = property.PropertyType.GetGenericArguments();
        var (requestType, responseType) = types switch
        {
            [var request, var response] => (request, response),
            [var request] => (request, typeof(StreamableResponseMetadata)),
            _ => throw new NotSupportedException($"Usupported arguments: {string.Join(',', types.AsEnumerable())}")
        };

        return CreateMetadata(property, basePattern, requestType, responseType);
    }

    static HttpEndpointMetadata CreateMetadata(PropertyInfo property, string basePattern, Type requestType, Type responseType)
        => typeof(ReflectionHttpEndpointMetadata<,>)
            .MakeGenericType(requestType, responseType)
            .GetConstructor()
            .Invoke([property, basePattern]) as HttpEndpointMetadata;

    static Type MakeHttpEndpointMetadata(HttpEndpointMetadata metadata)
        => typeof(HttpEndpointMetadata<,>).MakeGenericType(metadata.Request.BodyType, metadata.Response.BodyType);

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
