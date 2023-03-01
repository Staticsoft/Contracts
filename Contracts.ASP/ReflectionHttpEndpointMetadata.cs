using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public class ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> : HttpEndpointMetadata<RequestBody, ResponseBody>
    {
        static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

        readonly PropertyInfo Property;
        public string Pattern { get; }
        public RequestType RequestType { get; }

        public ReflectionHttpEndpointMetadata(PropertyInfo property)
        {
            Property = property;
            RequestType = GetRequestType(property);
            Pattern = GetPattern(property, RequestType);
        }

        static RequestType GetRequestType(PropertyInfo property)
            => property.PropertyType.GetGenericTypeDefinition() == ParametrizedHttpEndpointType
            ? RequestType.Parametrized
            : RequestType.Static;

        static string GetPattern(PropertyInfo property, RequestType type)
            => $"/{property.DeclaringType.Name}/{GetPatternRemainder(property, type)}";

        static string GetPatternRemainder(PropertyInfo property, RequestType type)
        {
            var endpoint = property.GetCustomAttribute<EndpointAttribute>();
            if (endpoint.Pattern != property.Name) return endpoint.Pattern;

            return GetDefaultPatternRemainder(property, type);
        }

        static string GetDefaultPatternRemainder(PropertyInfo property, RequestType type) => type switch
        {
            RequestType.Static => property.Name,
            RequestType.Parametrized => "{parameter}",
            _ => throw new NotSupportedException($"{nameof(RequestType)} {type} is not supported")
        };

        public Type RequestBodyType
            => typeof(RequestBody);

        public Type ResponseBodyType
            => typeof(ResponseBody);

        public T GetAttribute<T>() where T : Attribute
            => Property.GetCustomAttribute<T>();

        public override bool Equals(object obj) => obj switch
        {
            ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> metadata => Equals(metadata),
            _ => false
        };

        bool Equals(ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> metadata)
            => metadata.RequestBodyType == RequestBodyType
            && metadata.ResponseBodyType == ResponseBodyType;

        public override int GetHashCode()
            => HashCode.Combine(RequestBodyType, ResponseBodyType);

        public override string ToString()
            => $"{nameof(HttpEndpointMetadata)}<{RequestBodyType.Name}, {ResponseBodyType.Name}>";
    }
}
