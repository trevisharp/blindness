using System.Reflection;

namespace Blindness.Internal;

using Concurrency.Elements;

public record EventMatch(object Parent, PropertyInfo Field, EventElement EventObject);