/* Author:  Leonardo Trevisan Silio
 * Date:    25/07/2024
 */
using System;
using System.Linq;

namespace Blindness.Injection;

using Exceptions;

public class ConstructorDepFunction : DepFunction
{
    public override object Call(
        Type type,
        Func<Type, TypeList, DepFunction, object> depSys, 
        TypeList deepDeps)
    {
        try
        {
            var constructors = type.GetConstructors();
            var defaultConstructor = constructors
                .FirstOrDefault(c => c.GetParameters().Length == 0);
            if (constructors.Length > 1 && defaultConstructor is null)
                throw new ManyConcreteTypeException(type);
            
            var constructor = defaultConstructor ?? constructors[0];

            var data = constructor
                .GetParameters()
                .Select(p => p.ParameterType)
                .Select(t => depSys(t, deepDeps, this))
                .ToArray();
            
            var obj = constructor.Invoke(data);
            return obj;
        }
        catch (Exception ex)
        {
            throw new DependencyLoadingException(ex, type);
        }
    }
}