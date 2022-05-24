namespace Staticsoft.Contracts.ASP.Server
{
    public interface HttpRequestHandlerFactory
    {
        HttpRequestHandler<TRequest, TResponse> Create<TRequest, TResponse>();
    }
}
