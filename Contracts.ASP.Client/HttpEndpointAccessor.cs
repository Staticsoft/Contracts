using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Client
{
    public class HttpEndpointAccessor<TRequest, TResponse> : HttpEndpoint<TRequest, TResponse>
    {
        readonly HttpRequestExecutor Executor;
        readonly HttpResultHandler Handler;
        readonly HttpEndpointMetadata<TRequest, TResponse> Metadata;
        readonly EndpointRequestFactory Factory;
        readonly HttpResponseParser Parser;

        public HttpEndpointAccessor(
            HttpRequestExecutor executor,
            HttpResultHandler handler,
            HttpEndpointMetadata<TRequest, TResponse> metadata,
            EndpointRequestFactory factory,
            HttpResponseParser parser
        )
        {
            Executor = executor;
            Handler = handler;
            Metadata = metadata;
            Factory = factory;
            Parser = parser;
        }

        public async Task<TResponse> Execute(TRequest body)
        {
            var request = Factory.Create(Metadata, body);
            var response = await Executor.Execute(request);
            return Handler.Handle(new HttpResult<TResponse>(response, Parser));
        }
    }
}
