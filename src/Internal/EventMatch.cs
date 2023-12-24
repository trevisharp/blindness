using System.Reflection;

namespace Blindness.Internal;

using Concurrency.Elements;

public record EventMatch(object Parent, MemberInfo Field, EventElement EventObject);