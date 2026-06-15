using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Server;

public static class DependencyInjectionExtensions
{
    static readonly Type HttpRequestHandlerType = typeof(HttpRequestHandler);
    static readonly Type StreamableHttpRequestHandlerType = typeof(StreamableHttpRequestHandler);

    public static IServiceCollection UseServerAPI<TAPI>(
        this IServiceCollection services,
        IEndpointRegistrations<TAPI> registrations)
        where TAPI : class
        => services
            .AddSingleton<IEndpointRegistrations<TAPI>>(registrations)
            .AddScoped<HttpRequestHandler, EndpointRequestHandler>()
            .AddScoped<StreamableHttpRequestHandler, StreamableEndpointRequestHandler>()
            .AddScoped<HttpEndpointFactory, DependencyInjectionHttpRequestHandlerFactory>()
            .AddScoped<ParametrizedHttpEndpointFactory, DependencyInjectionParametrizedHttpRequestHandlerFactory>()
            .AddScoped<StreamableHttpEndpointFactory, DependencyInjectionStreamableHttpEndpointFactory>()
            .AddScoped<StreamableParametrizedHttpEndpointFactory, DependencyInjectionStreamableParametrizedHttpEndpointFactory>()
            .AddEndpoints(registrations);

    static IServiceCollection AddEndpoints<TAPI>(
        this IServiceCollection services,
        IEndpointRegistrations<TAPI> registrations)
    {
        registrations.Register(services);
        return services;
    }

    public static IApplicationBuilder UseServerAPIRouting<TAPI>(this IApplicationBuilder builder)
        where TAPI : class
        => builder.UseEndpoints(endpoints =>
        {
            var registrations = endpoints.ServiceProvider.GetRequiredService<IEndpointRegistrations<TAPI>>();
            foreach (var metadata in registrations.GetMetadata())
                endpoints.Map(metadata);
        });

    static void Map(this IEndpointRouteBuilder builder, ASP.HttpEndpointMetadata metadata)
        => builder.GetMapper(metadata.GetAttribute<EndpointAttribute>().Method)(
            metadata.Request.Pattern.Value,
            context => HandleRequest(context, metadata));

    static Func<string, RequestDelegate, IEndpointConventionBuilder> GetMapper(
        this IEndpointRouteBuilder builder, HttpMethod method) => method switch
    {
        HttpMethod.Get => builder.MapGet,
        HttpMethod.Post => builder.MapPost,
        HttpMethod.Put => builder.MapPut,
        HttpMethod.Delete => builder.MapDelete,
        _ => throw new Exception($"Unsupported method: {method}")
    };

    static async Task HandleRequest(HttpContext context, ASP.HttpEndpointMetadata metadata)
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
