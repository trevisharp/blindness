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
using Blindness;
using Blindness.Injection;

var obj = DependencySystem.Shared.GetConcrete<List<string>>();
obj.Add("Hello");
obj.Add("Blindness");
Console.WriteLine(obj); // [ Hello, Blindness ]

// Update current assembly any time
DependencySystem.Shared.UpdateAssembly(typeof(List<string>).Assembly);
// Missing a subtype of System.Collections.Generic.List`1[System.String] with concrete attribute.
var obj2 = DependencySystem.Shared.GetConcrete<List<string>>();

public class MyList : List<string>
{
    public override string ToString()
        => $"[ {string.Join(' ', this)} ]";
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

### 

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