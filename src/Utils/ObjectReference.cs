using System;
using System.Reflection;

namespace Blindness.Internal;

internal record ObjectReference(
    object ObjectValue,
    Type ObjectType,
    MemberInfo MemberInfo
);