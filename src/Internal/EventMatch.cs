using System.Reflection;

namespace Blindness.Internal;

using Concurrency.Elements;

internal record EventMatch(object Parent, PropertyInfo Field, EventElement EventObject);