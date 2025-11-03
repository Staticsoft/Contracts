using System;

namespace Staticsoft.Contracts.Abstractions;

[AttributeUsage(AttributeTargets.Property)]
public class EndpointBehaviorAttribute(
    int statusCode
) : Attribute
{
    public readonly int StatusCode = statusCode;
}
