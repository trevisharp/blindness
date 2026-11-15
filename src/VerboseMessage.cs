/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
namespace Blindness;

/// <summary>
/// Represents a Verbose message sent to the sinks. Text already has the group tabulation.
/// </summary>
public readonly record struct VerboseMessage(
    VerboseKind Kind,
    int Level,
    string Text,
    bool Inline
);
