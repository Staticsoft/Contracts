using System;

namespace Staticsoft.Contracts.Abstractions;

public class HttpResultHandlerException(
    int statusCode
) : Exception($"Unexpected status code received: {statusCode}")
{
    public readonly int StatusCode = statusCode;
}
