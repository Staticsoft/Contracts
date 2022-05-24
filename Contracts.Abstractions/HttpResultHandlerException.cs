using System;

namespace Staticsoft.Contracts.Abstractions
{
    public class HttpResultHandlerException : Exception
    {
        public readonly int StatusCode;

        public HttpResultHandlerException(int statusCode)
            : base($"Unexpected status code received: {statusCode}")
            => StatusCode = statusCode;
    }
}
