/* Author:  Leonardo Trevisan Silio
 * Date:    22/07/2024
 */
using System.Text;

namespace Blindness.Factory;

/// <summary>
/// A base class to all code builders.
/// </summary>
public abstract class CodeBuilder
{
    readonly StringBuilder builder = new();
    public StringBuilder Builder => builder;
}