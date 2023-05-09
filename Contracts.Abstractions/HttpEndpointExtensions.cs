using System.Threading.Tasks;

namespace Staticsoft.Contracts.Abstractions;

public static class HttpEndpointExtensions
{
    public static Task<TResponse> Execute<TResponse>(this HttpEndpoint<EmptyRequest, TResponse> endpoint)
        => endpoint.Execute(EmptyRequest.Empty);

    public static Task<TResponse> Execute<TRequest, TResponse>(this HttpEndpoint<TRequest, TResponse> endpoint)
        where TRequest : EmptyRequest, new()
        => endpoint.Execute(new());

    public static Task<TResponse> Execute<TResponse>(this ParametrizedHttpEndpoint<EmptyRequest, TResponse> endpoint, string parameter)
        => endpoint.Execute(parameter, EmptyRequest.Empty);

    public static Task<TResponse> Execute<TRequest, TResponse>(this ParametrizedHttpEndpoint<TRequest, TResponse> endpoint, string parameter)
        where TRequest : EmptyRequest, new()
        => endpoint.Execute(parameter, new());
}
