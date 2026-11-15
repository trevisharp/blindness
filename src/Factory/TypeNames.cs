/* Author:  Leonardo Trevisan Silio
 * Date:    14/11/2026
 */
using System;
using System.Linq;

namespace Blindness.Factory;

/// <summary>
/// Produces C# source names for runtime types.
/// </summary>
public static class TypeNames
{
    /// <summary>
    /// Get a fully qualified C# name for a type, usable in generated code.
    /// </summary>
    public static string CSharpName(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        if (type.IsByRef)
            return CSharpName(type.GetElementType()!);

        if (type.IsArray)
        {
            var commas = new string(',', type.GetArrayRank() - 1);
            return $"{CSharpName(type.GetElementType()!)}[{commas}]";
        }

        if (type.IsPointer)
            return $"{CSharpName(type.GetElementType()!)}*";

        if (type.IsGenericParameter)
            return type.Name;

        var allArgs = type.GetGenericArguments();
        return Qualified(type, allArgs, allArgs.Length);
    }

    /// <summary>
    /// Build the name of a type using the generic arguments that belong to it.
    /// </summary>
    static string Qualified(Type type, Type[] allArgs, int argsEnd)
    {
        var name = type.Name;
        var tick = name.IndexOf('`');
        var ownCount = 0;
        if (tick >= 0)
        {
            ownCount = int.Parse(name[(tick + 1)..]);
            name = name[..tick];
        }

        var ownArgs = allArgs[(argsEnd - ownCount)..argsEnd];
        var generic = ownCount == 0 ? "" :
            "<" + string.Join(", ", ownArgs.Select(CSharpName)) + ">";

        if (type.IsNested)
            return $"{Qualified(type.DeclaringType!, allArgs, argsEnd - ownCount)}.{name}{generic}";

        var ns = string.IsNullOrEmpty(type.Namespace) ? "" : type.Namespace + ".";
        return $"global::{ns}{name}{generic}";
    }
}
