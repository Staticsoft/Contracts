using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System.Threading.Tasks;

namespace Staticsoft.Contracts.ASP.Client;

public class HttpEndpointAccessor<RequestBody, ResponseBody> :
    HttpEndpoint<RequestBody, ResponseBody>,
    ParametrizedHttpEndpoint<RequestBody, ResponseBody>
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
