using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Internal;

using Exceptions;

internal record FromToExpression(ObjectReference From, ObjectReference To)
{
    internal static FromToExpression FromExpression<T, R>(
        Expression<Func<T, R>> expression, 
        object fromInstance, Type fromType
    )
    {
        ObjectReference from = null, to = null;
            
        var body = expression.Body;
        if (body.NodeType == ExpressionType.Convert)
        {
            var convertExp = body as UnaryExpression;
            body = convertExp.Operand;
        }

        var memberAccess = body as MemberExpression;
        if (memberAccess is null)
            return null;
        var member = memberAccess.Member;

        if (memberAccess.Expression is MemberExpression parentMemberAccess)
        {
            var parentField = parentMemberAccess?.Member;
            var parentConstant = parentMemberAccess.Expression as ConstantExpression;
            if (parentConstant is null)
                throw new InvalidBindingFormatException(
                    "Expected a x => obj.y format."
                );
            
            var propertyParent = parentConstant.Value;
            var (type, obj) = 
                parentField is PropertyInfo prop ? 
                    (prop.PropertyType, prop.GetValue(propertyParent)) :
                parentField is FieldInfo field ? 
                    (field.FieldType, field.GetValue(propertyParent)) :
                    throw new InvalidBindingFormatException(
                        "The obj in x => obj.y need be a Property or a Field"
                    );

            to = new(obj, type, member);
            
            var fromFieldName = expression
                .Parameters[0].Name;
            MemberInfo fromMember = fromType.GetProperty(fromFieldName);
            fromMember ??= fromType.GetField(fromFieldName);

            from = new(fromInstance, fromType, fromMember);
            
            if (from.MemberInfo is null)
                throw new MissingFieldException(fromFieldName, from.ObjectType);
            
            if (to.MemberInfo is null)
                throw new MissingFieldException("(unknown)", to.ObjectType);
        }

        if (memberAccess.Expression is ConstantExpression parentConstantAccess)
        {
            var propertyParent = parentConstantAccess.Value;
            
            var parentField = memberAccess?.Member;

            to = new(propertyParent, parentField.DeclaringType, parentField);

            var fromFieldName = expression
                .Parameters[0].Name;
            MemberInfo fromMember = fromType.GetProperty(fromFieldName);
            fromMember ??= fromType.GetField(fromFieldName);

            from = new(fromInstance, fromType, fromMember);
            
            if (from.MemberInfo is null)
                throw new MissingFieldException(fromFieldName, from.ObjectType);
            
            if (to.MemberInfo is null)
                throw new MissingFieldException("(unknown)", to.ObjectType);
        }

        return new(from, to);
    }
}