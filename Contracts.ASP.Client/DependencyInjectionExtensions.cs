using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Staticsoft.Contracts.ASP.Client;

public static class DependencyInjectionExtensions
{
    static readonly Type HttpEndpointType = typeof(HttpEndpoint<,>);
    static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);

    public static IServiceCollection UseClientAPI<TAPI>(this IServiceCollection services) where TAPI : class
        => services
            .AddSingleton<TAPI>()
            .AddGroups<TAPI>()
            .AddSingleton(HttpEndpointType, typeof(HttpEndpointAccessor<,>))
            .AddSingleton(ParametrizedHttpEndpointType, typeof(HttpEndpointAccessor<,>))
            .AddSingleton<HttpResultHandler, StatusCodeResultHandler>()
            .AddSingleton<EndpointRequestFactory, HttpEndpointRequestFactory>()
            .AddMetadata(HttpEndpointMetadataAccessor.GetMetadata(typeof(TAPI)));

    static IServiceCollection AddMetadata(this IServiceCollection services, IEnumerable<HttpEndpointMetadata> metadata)
        => HttpEndpointMetadataAccessor.AddMetadata(services, metadata);

    static IServiceCollection AddGroups<TAPI>(this IServiceCollection services)
        => GetGroupTypes(typeof(TAPI)).Aggregate(services, (services, type) => services.AddSingleton(type));

    static IEnumerable<Type> GetGroupTypes(Type type)
        => GetConstructorParametersTypes(type).SelectMany(parameterType =>
            IsConcreteType(parameterType)
            ? new[] { parameterType }.Concat(GetGroupTypes(parameterType))
            : Array.Empty<Type>()
        );

    static IEnumerable<Type> GetConstructorParametersTypes(Type type)
        => type.GetConstructor().GetParameters().Select(parameter => parameter.ParameterType);

    static bool IsConcreteType(Type type)
        => !type.IsInterface && !type.IsAbstract;
}
