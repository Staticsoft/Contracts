using Staticsoft.Contracts.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public class ReflectionHttpEndpointMetadata<TRequest, TResponse> : HttpEndpointMetadata<TRequest, TResponse>
    {
        static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

        readonly ParameterInfo Parameter;
        public string Path { get; }

        public ReflectionHttpEndpointMetadata(ParameterInfo parameter)
        {
            Parameter = parameter;
            Path = GetPath(parameter);
        }

        static string GetPath(ParameterInfo parameter)
            => $"/{parameter.Member.DeclaringType.Name}/{GetPathRemainder(parameter)}";

        static string GetPathRemainder(ParameterInfo parameter)
            => parameter.ParameterType.GetGenericTypeDefinition() == ParametrizedHttpEndpointType
            ? "{parameter}"
            : parameter.Name;

        public Type RequestType
            => typeof(TRequest);

        public Type ResponseType
            => typeof(TResponse);

        public T GetAttribute<T>() where T : Attribute
            => Parameter.GetCustomAttribute<T>();

        public override bool Equals(object obj) => obj switch
        {
            ReflectionHttpEndpointMetadata<TRequest, TResponse> metadata => Equals(metadata),
            _ => false
        };

        bool Equals(ReflectionHttpEndpointMetadata<TRequest, TResponse> metadata)
            => metadata.RequestType == RequestType
            && metadata.ResponseType == ResponseType;

        public override int GetHashCode()
            => HashCode.Combine(RequestType, ResponseType);

        public override string ToString()
            => $"{nameof(HttpEndpointMetadata)}<{RequestType.Name}, {ResponseType.Name}>";
    }
}
