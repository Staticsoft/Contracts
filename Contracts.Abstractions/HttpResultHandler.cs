using Staticsoft.HttpCommunication.Abstractions;

namespace Staticsoft.Contracts.Abstractions;

public interface HttpResultHandler
{
    TResponse Handle<TResponse>(HttpResult<TResponse> result);
}
