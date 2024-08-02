/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness;

/// <summary>
/// Attribute used to denote concrete nodes
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class ConcreteAttribute : Attribute;