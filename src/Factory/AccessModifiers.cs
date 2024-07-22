/* Author:  Leonardo Trevisan Silio
 * Date:    22/07/2024
 */
namespace Blindness.Factory;

/// <summary>
/// Represents a C# Access Modifier
/// </summary>
public class AccessModifier(string modifierCode)
{
    public static readonly AccessModifier File = new("file");
    public static readonly AccessModifier Public = new("public");
    public static readonly AccessModifier Private = new("private");
    public static readonly AccessModifier Internal = new("internal");
    public static readonly AccessModifier Protected = new("protected");
    public static readonly AccessModifier ProtectedInternal = new("protected internal");
    public static readonly AccessModifier PrivateProtected = new("private protected");

    public override string ToString()
        => modifierCode ?? string.Empty;
}