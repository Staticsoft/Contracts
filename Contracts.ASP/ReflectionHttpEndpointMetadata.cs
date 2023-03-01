using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public class ReflectionHttpEndpointMetadata<RequestBody, ResponseBody> : HttpEndpointMetadata<RequestBody, ResponseBody>
    {
        static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

        readonly ParameterInfo Parameter;
        public string Path { get; }
        public RequestType RequestType { get; }

        public ReflectionHttpEndpointMetadata(ParameterInfo parameter)
        {
            Parameter = parameter;
            RequestType = GetRequestType(parameter);
            Path = GetPath(parameter, RequestType);
        }

        static RequestType GetRequestType(ParameterInfo parameter)
            => parameter.ParameterType.GetGenericTypeDefinition() == ParametrizedHttpEndpointType
            ? RequestType.Parametrized
            : RequestType.Static;

        static string GetPath(ParameterInfo parameter, RequestType type)
            => $"/{parameter.Member.DeclaringType.Name}/{GetPathRemainder(parameter, type)}";

        static string GetPathRemainder(ParameterInfo parameter, RequestType type) => type switch
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
