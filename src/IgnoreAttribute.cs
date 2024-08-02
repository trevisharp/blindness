/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness;

/// <summary>
/// Attribute used to ignore interfaces in code generation step.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class IgnoreAttribute : Attribute;