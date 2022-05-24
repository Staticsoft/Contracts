using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.Contracts.Abstractions
{
    public class StatusCodeResultHandler : HttpResultHandler
    {
        public TResponse Handle<TResponse>(HttpResult<TResponse> result) => result.StatusCode switch
        {
            200 => result.Body,
            _ => throw new HttpResultHandlerException(result.StatusCode)
        };
    }
}
