/* Author:  Leonardo Trevisan Silio
 * Date:    22/07/2024
 */
namespace Blindness.Factory;

/// <summary>
/// A base class to all member code builders.
/// </summary>
public abstract class MemberBuilder(ClassBuilder parent)
{
    protected ClassBuilder parent = parent;

    protected virtual void Build() { }
    /// <summary>
    /// Append member to parent.
    /// </summary>
    public ClassBuilder AppendMember()
    {
        Build();
        return parent;
    }
}