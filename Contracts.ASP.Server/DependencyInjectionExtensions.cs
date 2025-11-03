using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server;

public static class DependencyInjectionExtensions
{
    static readonly Type HttpEndpointType = typeof(HttpEndpoint<,>);
    static readonly Type ParametrizedHttpEndpointType = typeof(ParametrizedHttpEndpoint<,>);
    static readonly Type StreamableHttpEndpointType = typeof(StreamableHttpEndpoint<>);
    static readonly Type StreamableParametrizedHttpEndpointType = typeof(StreamableParametrizedHttpEndpoint<>);
    static readonly Type HttpRequestHandlerType = typeof(HttpRequestHandler);
    static readonly Type StreamableHttpRequestHandlerType = typeof(StreamableHttpRequestHandler);

    public static IServiceCollection UseServerAPI<TAPI>(this IServiceCollection services, Assembly assembly)
        where TAPI : class
        => services
            .AddScoped<HttpRequestHandler, EndpointRequestHandler>()
            .AddScoped<StreamableHttpRequestHandler, StreamableEndpointRequestHandler>()
            .AddScoped<HttpEndpointFactory, DependencyInjectionHttpRequestHandlerFactory>()
            .AddScoped<ParametrizedHttpEndpointFactory, DependencyInjectionParametrizedHttpRequestHandlerFactory>()
            .AddScoped<StreamableHttpEndpointFactory, DependencyInjectionStreamableHttpEndpointFactory>()
            .AddScoped<StreamableParametrizedHttpEndpointFactory, DependencyInjectionStreamableParametrizedHttpEndpointFactory>()
            .AddEndpoints<TAPI>(HttpEndpointMetadataAccessor.GetMetadata(typeof(TAPI)), assembly);

    static IServiceCollection AddEndpoints<TAPI>(this IServiceCollection services, IEnumerable<HttpEndpointMetadata> metadata, Assembly assembly)
        where TAPI : class
        => HttpEndpointMetadataAccessor.AddMetadata(services, metadata)
            .RegisterEndpointsImplementations(metadata, GetEndpointsImplementations(assembly));

    static Dictionary<Type, Type> GetEndpointsImplementations(Assembly assembly)
        => assembly.GetTypes()
            .Where(type => type.GetInterfaces().Any(IsHttpEndpointInterface))
            .ToDictionary(type => type.GetInterfaces().Single(IsHttpEndpointInterface));

    static bool IsHttpEndpointInterface(Type interfaceType)
        => interfaceType.IsGenericTypeOf(HttpEndpointType)
        || interfaceType.IsGenericTypeOf(ParametrizedHttpEndpointType)
        || interfaceType.IsGenericTypeOf(StreamableHttpEndpointType)
        || interfaceType.IsGenericTypeOf(StreamableParametrizedHttpEndpointType);

    static IServiceCollection RegisterEndpointsImplementations(
        this IServiceCollection services,
        IEnumerable<HttpEndpointMetadata> metadatas,
        Dictionary<Type, Type> implementations
    )
        => services.RegisterEndpointsImplementationsIfAllEndpointsImplemented(GetNonImplementedEndpoints(metadatas, implementations), implementations);

    static IEnumerable<HttpEndpointMetadata> GetNonImplementedEndpoints(
        IEnumerable<HttpEndpointMetadata> metadatas,
        Dictionary<Type, Type> implementations
    )
        => metadatas.Where(metadata => NotImplementedEndpoint(implementations, metadata));

    static bool NotImplementedEndpoint(Dictionary<Type, Type> implementations, HttpEndpointMetadata metadata)
        => metadata.Request.Pattern.Type switch
        {
            PatternType.Static when metadata.Response.BodyType == typeof(StreamableResponseMetadata)
                => !implementations.ContainsKey(StreamableHttpEndpointType.MakeGenericType(metadata.Request.BodyType)),
            PatternType.Static
                => !implementations.ContainsKey(HttpEndpointType.MakeGenericType(metadata.Request.BodyType, metadata.Response.BodyType)),
            PatternType.Parametrized when metadata.Response.BodyType == typeof(StreamableResponseMetadata)
                => !implementations.ContainsKey(StreamableParametrizedHttpEndpointType.MakeGenericType(metadata.Request.BodyType)),
            PatternType.Parametrized
                => !implementations.ContainsKey(ParametrizedHttpEndpointType.MakeGenericType(metadata.Request.BodyType, metadata.Response.BodyType)),
            _ => throw new NotSupportedException($"{nameof(PatternType)} {metadata.Request.Pattern.Type} is not supported")
        };

    static IServiceCollection RegisterEndpointsImplementationsIfAllEndpointsImplemented<T>(
        this IServiceCollection services,
        IEnumerable<T> nonImplementedMetadatas,
        Dictionary<Type, Type> implementations
    )
        => nonImplementedMetadatas.Any()
        ? throw new Exception($"No endpoint implementation found for metadatas: {string.Join(", ", nonImplementedMetadatas)}")
        : implementations.Keys
            .Aggregate(services, (services, endpoint) => services.AddScoped(endpoint, implementations[endpoint]));

    public static IApplicationBuilder UseServerAPIRouting<TAPI>(this IApplicationBuilder builder)
        where TAPI : class
        => builder.UseEndpoints(ConfigureEndpoints<TAPI>);

    static void ConfigureEndpoints<TAPI>(IEndpointRouteBuilder builder)
        where TAPI : class
    {
        var metadatas = HttpEndpointMetadataAccessor.GetMetadata(typeof(TAPI));
        foreach (var metadata in metadatas)
        {
            builder.Map(metadata);
        }
    }

    static void Map(this IEndpointRouteBuilder builder, HttpEndpointMetadata metadata)
        => builder.GetMapper(metadata.GetAttribute<EndpointAttribute>().Method)(metadata.Request.Pattern.Value, (context) => HandleRequest(context, metadata));

    static Func<string, RequestDelegate, IEndpointConventionBuilder> GetMapper(this IEndpointRouteBuilder builder, HttpMethod method) => method switch
    {
        HttpMethod.Get => builder.MapGet,
        HttpMethod.Post => builder.MapPost,
        HttpMethod.Put => builder.MapPut,
        HttpMethod.Delete => builder.MapDelete,
        _ => throw new Exception($"Unsupported method: {method}")
    };

    static async Task HandleRequest(HttpContext context, HttpEndpointMetadata metadata)
    {
        var (handlerType, arguments) = metadata.Response.BodyType == typeof(StreamableResponseMetadata)
            ? (StreamableHttpRequestHandlerType, new Type[] { metadata.Request.BodyType })
            : (HttpRequestHandlerType, new Type[] { metadata.Request.BodyType, metadata.Response.BodyType });

        await (Task)handlerType
            .GetMethod("Execute")
            .MakeGenericMethod(arguments)
            .Invoke(context.RequestServices.GetRequiredService(handlerType), [context, metadata]);
    }
}
