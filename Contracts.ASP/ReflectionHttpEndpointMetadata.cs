using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public class ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> : HttpEndpointMetadata<RequestBody, ResponseBody>
    {
        static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

        readonly PropertyInfo Property;

        public RequestMetadata Request { get; }
        public ResponseMetadata Response { get; }

        public T GetAttribute<T>() where T : Attribute
            => Property.GetCustomAttribute<T>();

        public ReflectionHttpEndpointMetadata(PropertyInfo property, string basePattern)
        {
            Property = property;
            Request = GetRequestMetadata(property, basePattern);
            Response = GetResponseMetadata();
        }

        static RequestMetadata GetRequestMetadata(PropertyInfo property, string basePattern)
            => GetRequestMetadata(property, GetPatternType(property), basePattern);

        static RequestMetadata GetRequestMetadata(PropertyInfo property, PatternType patternType, string basePattern) => new()
        {
            BodyType = typeof(RequestBody),
            Pattern = new()
            {
                Value = GetPattern(property, basePattern, patternType),
                Type = patternType
            }
        };

        static ResponseMetadata GetResponseMetadata() => new()
        {
            BodyType = typeof(ResponseBody)
        };

        static PatternType GetPatternType(PropertyInfo property)
            => property.PropertyType.GetGenericTypeDefinition() == ParametrizedHttpEndpointType
            ? PatternType.Parametrized
            : PatternType.Static;

        static string GetPattern(PropertyInfo property, string basePattern, PatternType type)
            => $"{basePattern}/{GetEndpointPattern(property, type)}";

        static string GetEndpointPattern(PropertyInfo property, PatternType type)
        {
            var endpoint = property.GetCustomAttribute<EndpointAttribute>();
            if (endpoint.Pattern != property.Name) return endpoint.Pattern;

            return GetDefaultEndpointPattern(property, type);
        }

        static string GetDefaultEndpointPattern(PropertyInfo property, PatternType type) => type switch
        {
            PatternType.Static => property.Name,
            PatternType.Parametrized => "{parameter}",
            _ => throw new NotSupportedException($"{nameof(PatternType)} {type} is not supported")
        };

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
}
