/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Linq;

namespace Blindness.Core.Injections;

using Injection;
using Exceptions;

public class DepsDepFunction : DepFunction
{
    public override object Call(Type type, DependencySystem depSys, InjectionArgs args)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        var constructor = type.GetConstructors()
            .FirstOrDefault(c => c.GetParameters().Length == 0) 
            ?? throw new MissingEmptyConstructor(type);
        try
        {
            var obj = constructor.Invoke([]);

            var deps = type.GetMethod("Deps");
            if (deps is null)
                return obj;
            
            var data = 
                from parameter in deps.GetParameters()
                select parameter.ParameterType into paramType
                select depSys.Get(paramType, args);
            
            deps.Invoke(obj, [ ..data ]);

            return obj;
        }
        catch (Exception ex)
        {
            throw new ActivatorException(ex, type);
        }
    }
}