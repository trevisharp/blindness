/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System.Reflection;

namespace Blindness.Core.Concurrencies;

using Concurrency;

public record AssemblySignalArgs(Assembly NewAssembly, bool Success) : SignalArgs(Success);