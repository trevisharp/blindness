/* Author:  Leonardo Trevisan Silio
 * Date:    22/07/2024
 */
namespace Blindness.Factory;

/// <summary>
/// A base class to all member code builders.
/// </summary>
public class PropertyBuilder(ClassBuilder parent) : MemberBuilder(parent)
{
    string name = "MyProperty";
    string type = "string";
    string getCode = null;
    string setCode = null;
    AccessModifier accessModifier = AccessModifier.Public;

    public PropertyBuilder SetAccessModifier(AccessModifier accessModifier)
    {
        this.accessModifier = accessModifier;
        return this;
    }

    public PropertyBuilder SetName(string name)
    {
        this.name = name;
        return this;
    }

    public PropertyBuilder SetGetCode(string code)
    {
        getCode = code;
        return this;
    }

    public PropertyBuilder SetSetCode(string code)
    {
        setCode = code;
        return this;
    }

    public PropertyBuilder SetType(string type)
    {
        this.type = type;
        return this;
    }

    protected override void Build()
    {
        if (getCode is null && setCode is null) {
            parent.AddCodeLine($"{accessModifier} {type} {name} {{ get; set; }}");
            return;
        }

        parent.AddCodeLine($"{accessModifier} {type} {name}");
        parent.AddCodeLine("{");
        parent.AddScope();

        if (getCode?.Contains('\n') ?? false)
        {
            parent.AddCodeLine("get {");
            parent.AddScope();
            parent.AddCodeLine(getCode);
            parent.RemoveScope();
            parent.AddCodeLine("}");
        }
        else if (getCode is not null)
        {
            parent.AddCodeLine($"get => {getCode};");
        }

        if (setCode?.Contains('\n') ?? false)
        {
            parent.AddCodeLine("set {");
            parent.AddScope();
            parent.AddCodeLine(setCode);
            parent.RemoveScope();
            parent.AddCodeLine("}");
        }
        else if (setCode is not null)
        {
            parent.AddCodeLine($"set => {setCode};");
        }

        parent.RemoveScope();
        parent.AddCodeLine("}");
    }
}