/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
namespace Blindness.Factory;

using Implementations;

/// <summary>
/// The default implementer used by application.
/// </summary>
public class DefaultImplementer : Implementer
{
    public DefaultImplementer()
    {
        BaseInterface = typeof(INode);
        Implementations = [
            new ConstructorImplementation(),
            new ConcreteImplementation(),
            new DefaultUsingsImplementation(),
            new DepsImplementation(),
            new OnLoadImplementation(),
            new OnRunImplementation(),
            new DefaultPropertyImplementation()
        ];
    }
}