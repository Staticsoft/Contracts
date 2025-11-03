using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Client;

public class HttpEndpointAccessor<RequestBody, ResponseBody>(
    HttpRequestExecutor executor,
    HttpResultHandler handler,
    HttpEndpointMetadata<RequestBody, ResponseBody> metadata,
    EndpointRequestFactory factory,
    HttpResponseParser parser
) :
    HttpEndpoint<RequestBody, ResponseBody>,
    ParametrizedHttpEndpoint<RequestBody, ResponseBody>
{
    readonly HttpRequestExecutor Executor = executor;
    readonly HttpResultHandler Handler = handler;
    readonly HttpEndpointMetadata<RequestBody, ResponseBody> Metadata = metadata;
    readonly EndpointRequestFactory Factory = factory;
    readonly HttpResponseParser Parser = parser;

    public Task<ResponseBody> Execute(RequestBody body)
        => ExecuteRequest(Factory.Create(Metadata, Metadata.Request.Pattern.Value, body));

    public Task<ResponseBody> Execute(string parameter, RequestBody body)
        => ExecuteRequest(Factory.Create(Metadata, Metadata.Request.Pattern.Value.Replace("{parameter}", parameter), body));

    async Task<ResponseBody> ExecuteRequest(HttpRequest request)
    {
        var response = await Executor.Execute(request);
        return Handler.Handle(new HttpResult<ResponseBody>(response, Parser));
    }
}
