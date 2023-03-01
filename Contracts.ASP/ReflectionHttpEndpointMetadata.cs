using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public class ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> : HttpEndpointMetadata<RequestBody, ResponseBody>
    {
        static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

        readonly ParameterInfo Parameter;
        public string Pattern { get; }
        public RequestType RequestType { get; }

        public ReflectionHttpEndpointMetadata(ParameterInfo parameter)
        {
            Parameter = parameter;
            RequestType = GetRequestType(parameter);
            Pattern = GetPattern(parameter, RequestType);
        }

        static RequestType GetRequestType(ParameterInfo parameter)
            => parameter.ParameterType.GetGenericTypeDefinition() == ParametrizedHttpEndpointType
            ? RequestType.Parametrized
            : RequestType.Static;

        static string GetPattern(ParameterInfo parameter, RequestType type)
            => $"/{parameter.Member.DeclaringType.Name}/{GetPatternRemainder(parameter, type)}";

        static string GetPatternRemainder(ParameterInfo parameter, RequestType type)
        {
            var endpoint = parameter.GetCustomAttribute<EndpointAttribute>();
            if (endpoint.Pattern != parameter.Name) return endpoint.Pattern;

            return GetDefaultPatternRemainder(parameter, type);
        }

        static string GetDefaultPatternRemainder(ParameterInfo parameter, RequestType type) => type switch
        {
            RequestType.Static => parameter.Name,
            RequestType.Parametrized => "{parameter}",
            _ => throw new NotSupportedException($"{nameof(RequestType)} {type} is not supported")
        };

        public Type RequestBodyType
            => typeof(RequestBody);

        public Type ResponseBodyType
            => typeof(ResponseBody);

        public T GetAttribute<T>() where T : Attribute
            => Parameter.GetCustomAttribute<T>();

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
