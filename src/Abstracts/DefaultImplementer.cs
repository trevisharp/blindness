/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
namespace Blindness.Abstracts;

using Implementations;

/// <summary>
/// The default implementer used by application.
/// </summary>
public class DefaultImplementer : Implementer
{
    public DefaultImplementer()
    {
        this.BaseConcreteType = typeof(Node);
        this.Implementations.Add(new ConstructorImplementation());
        this.Implementations.Add(new ConcreteImplementation());
        this.Implementations.Add(new DefaultUsingsImplementation());
        this.Implementations.Add(new DepsImplementation());
        this.Implementations.Add(new OnLoadImplementation());
        this.Implementations.Add(new OnRunImplementation());
        this.Implementations.Add(new DefaultPropertyImplementation());
    }
}