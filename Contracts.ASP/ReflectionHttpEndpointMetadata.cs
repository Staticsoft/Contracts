using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public class ReflectionHttpEndpointMetadata<TRequest, TResponse> : HttpEndpointMetadata<TRequest, TResponse>
    {
        public HttpMethod Method { get; }
        public string Path { get; }

        public ReflectionHttpEndpointMetadata(ParameterInfo parameter)
        {
            Method = parameter.GetCustomAttribute<EndpointAttribute>().Method;
            Path = $"/{parameter.Name}";
        }

        public Type RequestType
            => typeof(TRequest);

        public Type ResponseType
            => typeof(TResponse);
    }
}
