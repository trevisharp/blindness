/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
namespace Blindness.Factory;

/// <summary>
/// Represents a generic extra code file.
/// </summary>
public abstract class ExtraFile
{
    public string FileName { get; set; }
    public bool Constant { get; set; }
    public abstract string Get();
}