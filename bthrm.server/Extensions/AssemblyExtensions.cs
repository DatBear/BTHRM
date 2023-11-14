using System.Reflection;

namespace bthrm.server.Extensions;

public static class AssemblyExtensions
{
    public static List<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
    {
        return assembly.GetTypesAssignableFrom(typeof(T));
    }

    public static List<Type> GetTypesAssignableFrom(this Assembly assembly, Type interfaceToCompare)
    {
        List<Type> types = new List<Type>();
        foreach (var type in assembly.DefinedTypes)
        {
            if (interfaceToCompare.IsAssignableFrom(type) && !type.IsGenericType && interfaceToCompare != type)
            {
                types.Add(type);
            }
        }

        return types;
    }
}