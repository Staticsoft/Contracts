using System.Threading.Tasks;

namespace Staticsoft.Contracts.Abstractions
{
    public static class HttpEndpointExtensions
    {
        public static Task<TResponse> Execute<TResponse>(this HttpEndpoint<EmptyRequest, TResponse> endpoint)
            => endpoint.Execute(EmptyRequest.Empty);
    }
}
