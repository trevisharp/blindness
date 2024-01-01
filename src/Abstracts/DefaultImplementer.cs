/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Abstracts;

public class DefaultImplementer : Implementer
{
    public DefaultImplementer()
    {
        this.BaseConcreteType = typeof(Node);
    }
}