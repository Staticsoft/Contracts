using System;
using System.Linq;
using System.Reflection;

namespace Staticsoft.Contracts.ASP
{
    public static class TypeExtensions
    {
        public static ConstructorInfo GetConstructor(this Type type)
            => Try.Return(() => type.GetConstructors().Single())
                .On<InvalidOperationException>((_) => new ArgumentException($"{type.Name} should have single constructor"))
                .Result();

        public static bool IsGenericTypeOf(this Type type, Type genericType)
            => type.IsGenericType
            && type.GetGenericTypeDefinition() == genericType;
    }
}
