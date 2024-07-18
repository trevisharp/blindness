using System.Reflection;

namespace Blindness.Internal;

using Blindness;

internal record EventMatch(
    object Parent,
    PropertyInfo Field,
    EventElement EventObject
);