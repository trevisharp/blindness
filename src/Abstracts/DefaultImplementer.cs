/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
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
        BaseConcreteType = typeof(Node);
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