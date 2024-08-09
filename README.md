<p align="center">
  <img width="70%" src="res/logo.svg">
</p>

Blindness is a framework for you to build your frameworks without seeing the details.

# Table of Contents

 - [Overview](#overview)
 - [How to install](#how-to-install)
 - [Learn by examples](#learn-by-examples)
 - [Versions](#versions)

# Overview

Blidness will control your dependency injection system, your state management, your execution flow, your component oriented structure and your event system to allows you to concentrate where it really matters.

# How to install

```bash
dotnet new classlib # Create your library (can be a console or other option too)
dotnet add package Blindness # Install Blindness
```

# Learn by examples

### Using Blindness.Concurrency

```cs
using System;
using System.Threading;
using System.Collections.Concurrent;

using Blindness;
using Blindness.Concurrency;

// Create and start a default model
// You can create your own model
var model = new DefaultModel();
model.Run(new Consumer(model));
model.Start();

// Use BaseAsyncElement to create your async elements...
public class Consumer(IAsyncModel model) : BaseAsyncElement(model) 
{
    bool isRunning = false;
    public override void Pause() { /* ... */ }

    public override void Resume() { /* ... */ }

    public override void Run()
    {
        int sum = 0;
        int count = 0;
        var queue = new ConcurrentQueue<int>();

        var generator = new DataGenerator(queue);
        Model.Run(generator);

        isRunning = true;
        while (isRunning)
        {
            if (queue.TryDequeue(out var value))
            {
                sum += value;
                count++;
                Console.WriteLine(count);
            }

            if (queue.IsEmpty)
            {
                Console.WriteLine("Waiting...");
                // wait a signal from another element
                generator.Wait();
                Console.WriteLine("Running again!");
            }
        }
    }

    public override void Stop() { /* ... */ }
}

// ...or implements AsyncElement from scratch
public class DataGenerator(ConcurrentQueue<int> queue) : IAsyncElement
{
    public IAsyncModel Model => throw new NotImplementedException();
    readonly AutoResetEvent signal = new(false);
    public event Action<IAsyncElement, SignalArgs> OnSignal;
    public void Pause() { /* ... */ }
    public void Resume() { /* ... */ }
    public void Run() {
        while (true)
        {
            for (int i = 0; i < Random.Shared.Next(10); i++)
                queue.Enqueue(Random.Shared.Next());
            signal.Set();
            Thread.Sleep(Random.Shared.Next(400, 1000));
        }
    }
    public void Stop() { /* ... */ }
    public void Wait()
        => signal.WaitOne();
}
```

### Using LoopAsyncElement and other built-in elements

```cs
using System;
using System.Threading;

using Blindness;
using Blindness.Concurrency;
using Blindness.Concurrency.Elements;

var model = new DefaultModel();

var component = new MyComponent(model);
model.Run(component);

var myDelay = new DelayAsyncElement(model, 2, component.Stop);
model.Run(myDelay);

// Stop myDelay for 2 seconds extending MyComponent lifetime
model.Run(new DelayAsyncElement(model, 1, myDelay.Pause));
model.Run(new DelayAsyncElement(model, 3, myDelay.Resume));

model.Start(); // App running for 4 seconds

public class MyComponent(IAsyncModel model) : LoopAsyncElement(model)
{
    DateTime start;
    protected override void OnInit()
    {
        start = DateTime.Now;
    }

    // Run in loop
    protected override void OnRun()
    {
        var time = DateTime.Now - start;
        string message = time.TotalSeconds.ToString();
        Thread.Sleep(40);
        Console.Clear();
        Console.WriteLine(message);
    }

    protected override void OnStop()
    {
        Console.WriteLine("Goodbye!");
    }
}

```

### Using Dependency Injection System with Blindness.Injection

```cs
using Blindness.Injection;

var obj = DependencySystem.Shared.Get<A>();
// C Created!
// A
Console.WriteLine(obj);
var list = DependencySystem.Shared.Get<List<int>>();
// System.Collections.Generic.List`1[System.Int32]
Console.WriteLine(list);

public class A(B b);

public class B(C c);

public class C
{
    public C()
        => Console.WriteLine("C Created!");
}
```

### Get Concrete types from Abstracts and Filter types

```cs
using Blindness.Injection;

var obj = DependencySystem.Shared.Get<A>([
    // Exists a conflict between classes C and D because both implements A
    // but this following filter choose only classes that inheriths B class
    // solving the conflict.
    // You can use built-in filters ou implements your own.
    BaseTypeFilter.ByBaseType(typeof(B))
]);
Console.WriteLine(obj); // C object here

public interface A;
public abstract class B;
public class C : B, A;
public class D : A;
```

### Customize everything and control dependency injection

```cs
using System.Linq;
using System.Reflection;
using Blindness.Injection;

var obj = DependencySystem.Shared.Get<Base>(
    new MyDepFunction(), [ new MyFilter() ]
);

public class DependencyAttribute : Attribute;

public abstract class Base;

[Dependency]
public class Concrete : Base
{
    public void Deps(List<int> list)
    {
        Console.WriteLine("I received a list!");
    }
}

public class MyFilter : BaseTypeFilter
{
    public override bool Filter(Type type)
    {
        return type.GetCustomAttribute(typeof(DependencyAttribute)) is not null;
    }
}

public class MyDepFunction : DepFunction
{
    public override object Call(Type type,
        DependencySystem depSys, InjectionArgs args)
    {
        var constructors = type.GetConstructors();
        var constructor = constructors
            .FirstOrDefault(c => c.GetParameters().Length == 0);

        var obj = constructor.Invoke([]);
        var deps = type.GetMethod("Deps");
        if (deps is null)
            return obj;
        
        var data = deps
            .GetParameters()
            .Select(p => p.ParameterType)
            .Select(t => depSys.Get(t, args))
            .ToArray();
        deps.Invoke(obj, data);

        return obj;
    }
}
```

### Generate code easily with Blindness.Factory

```cs
using Blindness.Factory;

var myClass = new ClassBuilder();
var code = myClass
    .SetClassName("TextList")
    .AddUsing("System.Collections.Generic")
    .AddBaseType("List<string>")
    .AddCodeLine("// Get total character count from this collection")
    .CreateProperty()
        .SetType("int")
        .SetName("Characters")
        .SetGetCode("this.Sum(c => c.Length);")
    .AppendMember()
    .Build();
```
Generate the following code:
```cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if    
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;

public partial class TextList : List<string>
{
    // Get total character count from this collection
    public int Characters
    {
        get => this.Sum(c => c.Length);;
    }

}
```

### Automatically implemente yours interfaces with Blindness.Factory

```cs
using Blindness;
using Blindness.Factory;
using System.Reflection;

var implmenter = new MyImplementer();
implmenter.Implement();

[Ignore]
public interface IBase
{
    void Show();
}

public interface MyRealCode : IBase
{
    void OnShow()
        => Console.WriteLine("Blindness is cool!");
}

public class MyImplementation : Implementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName,
        Type implementedType, 
        List<PropertyInfo> properties,
        List<MethodInfo> methods)
    {
        builder
            .SetClassName($"{implementedType}Concrete")
            .AddBaseType(implementedType.Name)
            .AddUsing("System")
            .AddCodeLine(
                """
                public void OnShow()
                    => ((MyRealCode)this).OnShow();
                
                public void Show()
                {
                    Console.WriteLine("Starting the show...");
                    OnShow();
                }
                """
            );
    }
}

public class MyImplementer : Implementer
{
    public MyImplementer()
    {
        BaseInterface = typeof(IBase);
        Implementations = [ new MyImplementation() ];
    }
}
```

Generate the following file:

```cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

public partial class MyRealCodeConcrete : MyRealCode
{
	public void OnShow()
	    => ((MyRealCode)this).OnShow();
	
	public void Show()
	{
	    Console.WriteLine("Starting the show...");
	    OnShow();
	}

}
```

### Use Boxing easily with events using Blindness.Bind

```cs
using Blindness.Bind;

var myBox = new Box<int>();
myBox.Place(8);
myBox.OnChange += e =>
{
    Console.WriteLine("The value changed!");
    if (e.NewValue > e.OldValue)
        Console.WriteLine("The value incresead!");
};

var component = new MyComponent(myBox);
component.Run();
Console.WriteLine(myBox.Open());
// output:
// The value changed!
// The value incresead!
// The value changed!  
// 8

public class MyComponent(IPlaceable<int> reference)
{
    public void Run()
    {
        reference.Place(8);
        reference.Place(12);
        reference.Place(8);
    }
}
```

### Use Binding to manage Box smartly

```cs
using Blindness;
using Blindness.Bind;
using static Blindness.Bind.Binding;

MyComponent a = new();
MyComponent b = new();

// b.List = [1, 2, 3]
b.List = [1, 2, 3];
// a.List is binded to b.List and recive the value
// from the dominant type (b) so a.List = b.List = [ 1, 2, 3 ]
Bind(() => a.List == b.List);
// b.List = [], but a.List and b.List are binded so a.List = []
b.List = [];
// a.List = [ 2 ] so b.List = [ 2 ]
a.List.Add(2);

show(a.List); // [ 2 ]
show(b.List); // [ 2 ]

void show(List<int> list)
    => Verbose.Info($"[ {string.Join(", ", list)} ]");

public class MyComponent
{   
    // Create a Bindable Property
    [Binding]
    public List<int> List
    {
        get => Get(this).Open<List<int>>(nameof(List));
        set => Get(this).Place(nameof(List), value);
    }
}
```

### Bind non-Binding properties

```cs
using Blindness.Bind;
using static Blindness.Bind.Binding;

MyComponent a = new();

int value = 8;
Bind(() => a.Value == value); // Bind a.Value to value, so a.Value equals to 8 now
value += 2; // a.Value = value = 10
a.Value += 2; // a.Value = value = 12
Verbose.Info(value); // 12
Verbose.Info(a.Value); // 12

public class MyComponent
{   
    [Binding]
    public int Value
    {
        get => Get(this).Open<int>(nameof(Value));
        set => Get(this).Place(nameof(Value), value);
    }
}
```

### Bind many types of expressions

```cs
using Blindness;
using Blindness.Bind;
using static Blindness.Bind.Binding;

MyComponent index = new();
MyComponent oddIndex = new();
MyComponent oddValue = new();

List<int> list = [ 9, 6, 5, 11, 4, 3 ];

Bind(() => oddIndex.Value == 2 * index.Value + 1);
Bind(() => oddValue.Value == list[oddIndex.Value]);

int lastOddIndex = 
    list.Count % 2 == 0 ? 
    list.Count - 1 : list.Count;
oddIndex.Value = lastOddIndex;
int lastIndex = index.Value;

int sum = 0;
for (index.Value = 0; index.Value <= lastIndex; index.Value++)
    sum += oddValue.Value;
Verbose.Success(sum);

public class MyComponent
{
    [Binding]
    public int Value
    {
        get => Get(this).Open<int>(nameof(Value));
        set => Get(this).Place(nameof(Value), value);
    }
}
```

# Versions

### Blindness v3.0.0

 - ![](https://img.shields.io/badge/update-blue) Refactor all API to be more consistent and improve legibility.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Several Bug fix in all systems.

### Blindness v2.0.0

 - ![](https://img.shields.io/badge/update-blue) Comments and documentation improvements. 
 - ![](https://img.shields.io/badge/update-blue) Improvement in customization of Code Generation System.
 - ![](https://img.shields.io/badge/update-blue) Geral improvements in HotReload System.
 - ![](https://img.shields.io/badge/update-blue) Customization system for general App.
 - ![](https://img.shields.io/badge/update-blue) Update validation code to test if is needed regenerate.

### Blindness v1.0.0

 - ![](https://img.shields.io/badge/new-green) Code generation added.
 - ![](https://img.shields.io/badge/new-green) Concrete node generations system.
 - ![](https://img.shields.io/badge/new-green) HotReload system added.

### Blindness v0.5.0

 - ![](https://img.shields.io/badge/new-green) Event system added.

### Blindness v0.4.0

 - ![](https://img.shields.io/badge/new-green) Parallel and Async Node Actions System added.
 - ![](https://img.shields.io/badge/new-green) Special Nodes added.

### Blindness v0.3.0

 - ![](https://img.shields.io/badge/new-green) Basic flow control system.
 - ![](https://img.shields.io/badge/new-green) Verbose and exception system.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Many Bugs solved in field initialization.

### Blindness v0.2.0

 - ![](https://img.shields.io/badge/update-blue) Node creation systax changed to use interfaces.
 - ![](https://img.shields.io/badge/new-green) Dependency Injection System added.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Many Bugs solved in Binding System.

### Blindness v0.1.0

 - ![](https://img.shields.io/badge/new-green) Binding System added.