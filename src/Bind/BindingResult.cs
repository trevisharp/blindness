/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a result of a binding operation.
/// </summary>
public class BindingResult
{
    public readonly static BindingResult Unsuccesfull = new();

    public static BindingResult Successfull(object mainBox, Binding binding = null)
        => new() {
            Success = true,
            MainBox = mainBox,
            BoxBinding = binding
        };

    /// <summary>
    /// Get or Set if the binding operation result in a success.
    /// </summary>
    /// <value></value>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Get or Set the main Box from binding operation.
    /// </summary>
    public object MainBox { get; set; } = null;

    /// <summary>
    /// Get or Set the main Box's binding.
    /// </summary>
    public Binding BoxBinding { get; set; } = null;
}