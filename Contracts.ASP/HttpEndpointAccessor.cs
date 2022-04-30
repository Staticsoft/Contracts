using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP
{
    public class HttpEndpointAccessor<TRequest, TResponse> : HttpEndpoint<TRequest, TResponse>
    {
        readonly Http Http;
        readonly HttpResultHandler Handler;
        readonly HttpEndpointMetadata<TRequest, TResponse> Metadata;

        public HttpEndpointAccessor(Http http, HttpResultHandler handler, HttpEndpointMetadata<TRequest, TResponse> metadata)
        {
            Http = http;
            Handler = handler;
            Metadata = metadata;
        }

        public async Task<TResponse> Execute(TRequest request)
        {
            var response = await Http.Request<TResponse>(Metadata.Method, Metadata.Path, request);
            return Handler.Handle(response);
        }
    }
}
