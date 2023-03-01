using Staticsoft.HttpCommunication.Abstractions;
using System;
using System.Runtime.CompilerServices;

namespace Staticsoft.Contracts.Abstractions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EndpointAttribute : Attribute
    {
        public readonly HttpMethod Method;
        public readonly string Pattern;

        public EndpointAttribute(HttpMethod method, [CallerMemberName] string pattern = "")
            => (Method, Pattern)
            = (method, pattern);
    }
}
