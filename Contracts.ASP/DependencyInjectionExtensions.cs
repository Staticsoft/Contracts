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

namespace Staticsoft.Contracts.ASP
{
    public static class DependencyInjectionExtensions
    {
        static readonly Type HttpEndpointType = typeof(HttpEndpoint<,>);
        static readonly Type HttpRequestHandlerType = typeof(HttpRequestHandler);

        public static IServiceCollection UseServerAPI<TAPI>(this IServiceCollection services, Assembly assembly)
            where TAPI : class
            => services
                .RegisterEndpointsImplementations<TAPI>(GetEndpointsImplementations(assembly))
                .AddSingleton<HttpRequestHandler, EndpointRequestHandler>()
                .AddSingleton(typeof(HttpRequestHandler<,>), typeof(EndpointRequestHandler<,>));

        static Dictionary<Type, Type> GetEndpointsImplementations(Assembly assembly)
            => assembly.GetTypes()
                .Where(type => type.GetInterfaces().Any(IsEndpointType))
                .ToDictionary(type => HttpEndpointType.MakeGenericType(type.GetInterfaces().Single(IsEndpointType).GetGenericArguments()));

        static IServiceCollection RegisterEndpointsImplementations<TAPI>(this IServiceCollection services, Dictionary<Type, Type> implementations)
            where TAPI : class
            => implementations.Keys
                .Aggregate(services, (services, endpoint) => services.AddSingleton(endpoint, implementations[endpoint]));

        public static IApplicationBuilder UseServerAPIRouting<TAPI>(this IApplicationBuilder builder)
            where TAPI : class
            => builder.UseEndpoints(ConfigureEndpoints<TAPI>);

        static void ConfigureEndpoints<TAPI>(IEndpointRouteBuilder builder)
            where TAPI : class
        {
            var registrations = GetMetadata(typeof(TAPI));
            foreach (var metadata in registrations)
            {
                builder.Map(metadata);
            }
        }

        static void Map(this IEndpointRouteBuilder builder, HttpEndpointMetadata metadata)
            => builder.GetMapper(metadata.Method)(metadata.Path, (context) => HandleRequest(context, metadata.RequestType, metadata.ResponseType));

        static Func<string, RequestDelegate, IEndpointConventionBuilder> GetMapper(this IEndpointRouteBuilder builder, HttpMethod method) => method switch
        {
            HttpMethod.Get => builder.MapGet,
            HttpMethod.Post => builder.MapPost,
            HttpMethod.Put => builder.MapPut,
            HttpMethod.Delete => builder.MapDelete,
            _ => throw new Exception($"Unsupported method: {method}")
        };

        static async Task HandleRequest(HttpContext context, Type requestType, Type responseType)
            => await (Task)HttpRequestHandlerType
                .GetMethod(nameof(HttpRequestHandler.Execute))
                .MakeGenericMethod(requestType, responseType)
                .Invoke(context.RequestServices.GetRequiredService(HttpRequestHandlerType), new[] { context });

        public static IServiceCollection UseClientAPI<TAPI>(this IServiceCollection services)
            where TAPI : class
        {
            services
                .AddSingleton<TAPI>()
                .AddSingleton(HttpEndpointType, typeof(HttpEndpointAccessor<,>))
                .AddGroups<TAPI>()
                .AddSingleton<HttpResultHandler, StatusCodeResultHandler>();

            foreach (var metadata in GetMetadata(typeof(TAPI)))
            {
                services.AddSingleton(
                    typeof(HttpEndpointMetadata<,>).MakeGenericType(metadata.RequestType, metadata.ResponseType),
                    metadata
                );
            }
            return services;
        }

        static IServiceCollection AddGroups<TAPI>(this IServiceCollection services)
            => GetGroupTypes(typeof(TAPI)).Aggregate(services, (services, type) => services.AddSingleton(type));

        static IEnumerable<Type> GetGroupTypes(Type type)
        {
            foreach (var parameterType in GetConstructorParametersTypes(type))
            {
                if (IsConcreteType(parameterType))
                {
                    yield return parameterType;
                    foreach (var innerType in GetGroupTypes(parameterType))
                    {
                        yield return innerType;
                    }
                }
            }
        }

        static IEnumerable<Type> GetEndpointTypes(Type type)
        {
            foreach (var parameterType in GetConstructorParametersTypes(type))
            {
                if (IsEndpointType(parameterType))
                {
                    yield return parameterType;
                }
                else if (IsConcreteType(parameterType))
                {
                    foreach (var innerType in GetEndpointTypes(parameterType))
                    {
                        yield return innerType;
                    }
                }
            }
        }

        static IEnumerable<Type> GetConstructorParametersTypes(Type type)
            => GetConstructor(type).GetParameters().Select(parameter => parameter.ParameterType);

        static bool IsConcreteType(Type type)
            => !type.IsInterface && !type.IsAbstract;

        static IEnumerable<HttpEndpointMetadata> GetMetadata(Type type)
            => GetConstructor(type).GetParameters().SelectMany(GetMetadata);

        static IEnumerable<HttpEndpointMetadata> GetMetadata(ParameterInfo parameter)
        {
            if (IsEndpointType(parameter.ParameterType))
            {
                var (requestType, responseType) = GetRequestResponseTypes(parameter.ParameterType);
                yield return CreateMetadata(parameter, requestType, responseType);
            }
            else
            {
                foreach (var registration in GetMetadata(parameter.ParameterType))
                {
                    yield return registration;
                }
            }
        }

        static HttpEndpointMetadata CreateMetadata(ParameterInfo parameter, Type requestType, Type responseType)
            => GetConstructor(typeof(ReflectionHttpEndpointMetadata<,>).MakeGenericType(requestType, responseType))
                .Invoke(new[] { parameter }) as HttpEndpointMetadata;

        static (Type, Type) GetRequestResponseTypes(Type type)
            => GetRequestResponseTypes(type.GetGenericArguments());

        static (Type, Type) GetRequestResponseTypes(Type[] types)
            => (types.First(), types.Last());

        static bool IsEndpointType(Type type)
            => type.IsInterface
            && type.IsGenericType
            && type.GetGenericTypeDefinition() == HttpEndpointType;

        static ConstructorInfo GetConstructor(Type type)
            => Try.Return(() => type.GetConstructors().Single())
                .On<InvalidOperationException>((_) => new ArgumentException($"{type.Name} should have only single constructor"))
                .Result();
    }
}
