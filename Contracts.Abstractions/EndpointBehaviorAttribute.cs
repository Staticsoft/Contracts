using System;

namespace Staticsoft.Contracts.Abstractions;

[AttributeUsage(AttributeTargets.Property)]
public class EndpointBehaviorAttribute : Attribute
{
    public readonly int StatusCode;

    public EndpointBehaviorAttribute(int statusCode)
        => StatusCode = statusCode;
}
