/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
namespace Blindness.Abstracts;

public abstract class ExtraFile
{
    public string FileName { get; set; }
    public bool Constant { get; set; }
    public abstract string Get();
}