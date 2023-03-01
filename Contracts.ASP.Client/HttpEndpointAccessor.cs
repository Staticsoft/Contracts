using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Client
{
    public class HttpEndpointAccessor<RequestBody, ResponseBody> : HttpEndpoint<RequestBody, ResponseBody>
    {
        readonly HttpRequestExecutor Executor;
        readonly HttpResultHandler Handler;
        readonly HttpEndpointMetadata<RequestBody, ResponseBody> Metadata;
        readonly EndpointRequestFactory Factory;
        readonly HttpResponseParser Parser;

        public HttpEndpointAccessor(
            HttpRequestExecutor executor,
            HttpResultHandler handler,
            HttpEndpointMetadata<RequestBody, ResponseBody> metadata,
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

        public async Task<ResponseBody> Execute(RequestBody body)
        {
            var request = Factory.Create(Metadata, body);
            var response = await Executor.Execute(request);
            return Handler.Handle(new HttpResult<ResponseBody>(response, Parser));
        }
    }
}
