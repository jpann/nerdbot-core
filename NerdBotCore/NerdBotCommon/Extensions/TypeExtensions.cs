using System;
using System.Linq;

namespace NerdBotCommon.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }
    }
}
