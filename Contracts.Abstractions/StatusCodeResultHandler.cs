using Staticsoft.HttpCommunication.Abstractions;
using System;

namespace Staticsoft.Contracts.Abstractions
{
    public class StatusCodeResultHandler : HttpResultHandler
    {
        public TResponse Handle<TResponse>(HttpResult<TResponse> result) => result.StatusCode switch
        {
            200 => result.Body,
            _ => throw new Exception($"Unexpected status code received: {result.StatusCode}")
        };
    }
}
