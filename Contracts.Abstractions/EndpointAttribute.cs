using Staticsoft.HttpCommunication.Abstractions;
using System;

namespace Staticsoft.Contracts.Abstractions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class EndpointAttribute : Attribute
    {
        public readonly HttpMethod Method;

        public EndpointAttribute(HttpMethod method)
            => Method = method;
    }
}
