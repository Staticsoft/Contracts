using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP;

public static class ReflectionMetadata
{
    public static RequestMetadata BuildRequestMetadata<RequestBody>(PropertyInfo property, string basePattern, Type parametrizedEndpointType)
        => GetRequestMetadata<RequestBody>(property, GetPatternType(property, parametrizedEndpointType), basePattern);

    static RequestMetadata GetRequestMetadata<RequestBody>(PropertyInfo property, PatternType patternType, string basePattern)
        => new()
        {
            BodyType = typeof(RequestBody),
            Pattern = new()
            {
                Value = GetPattern(property, basePattern, patternType),
                Type = patternType
            }
        };

    static PatternType GetPatternType(PropertyInfo property, Type parametrizedEndpointType)
    {
        var genericType = property.PropertyType.GetGenericTypeDefinition();
        return genericType == parametrizedEndpointType
            ? PatternType.Parametrized
            : PatternType.Static;
    }

    static string GetPattern(PropertyInfo property, string basePattern, PatternType type)
        => $"{basePattern}/{GetEndpointPattern(property, type)}/";

    static string GetEndpointPattern(PropertyInfo property, PatternType type)
    {
        var endpoint = property.GetCustomAttribute<EndpointAttribute>();
        if (endpoint != null && endpoint.Pattern != property.Name) return endpoint.Pattern;

        return GetDefaultEndpointPattern(property, type);
    }

    static string GetDefaultEndpointPattern(PropertyInfo property, PatternType type) => type switch
    {
        PatternType.Static => property.Name,
        PatternType.Parametrized => "{parameter}",
        _ => throw new NotSupportedException($"{nameof(PatternType)} {type} is not supported")
    };
}
