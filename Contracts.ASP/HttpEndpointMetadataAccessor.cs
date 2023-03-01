using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public static class HttpEndpointMetadataAccessor
    {
        static readonly Type HttpEndpointType = typeof(HttpEndpoint<,>);
        static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

        public static IServiceCollection AddMetadata(IServiceCollection services, IEnumerable<HttpEndpointMetadata> metadata)
        {
            var duplicates = GetDuplicates(metadata);
            if (duplicates.Any())
            {
                throw new Exception($"Duplicate metadata: {string.Join(", ", duplicates)}");
            }
            return metadata.Aggregate(services, (services, metadata) => services.AddSingleton(MakeHttpEndpointMetadata(metadata), metadata));
        }

        public static IEnumerable<HttpEndpointMetadata> GetMetadata(Type type)
            => type.GetProperties().SelectMany(GetMetadata);

        static IEnumerable<HttpEndpointMetadata> GetDuplicates(IEnumerable<HttpEndpointMetadata> metadatas)
        {
            var set = new HashSet<HttpEndpointMetadata>();
            foreach (var metadata in metadatas)
            {
                if (!set.Add(metadata))
                {
                    yield return metadata;
                }
            }
        }

        static IEnumerable<HttpEndpointMetadata> GetMetadata(PropertyInfo property)
            => property.PropertyType.IsGenericTypeOf(HttpEndpointType)
            || property.PropertyType.IsGenericTypeOf(ParametrizedHttpEndpointType)
            ? new[] { CreateMetadata(property) }
            : GetMetadata(property.PropertyType);

        static HttpEndpointMetadata CreateMetadata(PropertyInfo property)
        {
            var (requestType, responseType) = GetRequestResponseTypes(property.PropertyType);
            return CreateMetadata(property, requestType, responseType);
        }

        static (Type, Type) GetRequestResponseTypes(Type type)
            => GetRequestResponseTypes(type.GetGenericArguments());

        static (Type, Type) GetRequestResponseTypes(Type[] types)
            => (types.First(), types.Last());

        static HttpEndpointMetadata CreateMetadata(PropertyInfo property, Type requestType, Type responseType)
            => typeof(ReflectionHttpEndpointMetadata<,>).MakeGenericType(requestType, responseType).GetConstructor()
                .Invoke(new[] { property }) as HttpEndpointMetadata;

        static Type MakeHttpEndpointMetadata(HttpEndpointMetadata metadata)
            => typeof(HttpEndpointMetadata<,>).MakeGenericType(metadata.RequestBodyType, metadata.ResponseBodyType);
    }
}
