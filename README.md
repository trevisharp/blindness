# Blindness

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

### Use Blindness.Core to use all framework resources...

```cs
App.Start<Example>();

// Create interfaces that will be autoimplementend
public interface Example : INode
{
    string Value { get; set; }

    void OnLoad()
    {
        // This value is only setted on first run
        Value = "Hello, Blindness!";

        // Modify and test Hot Reload
        // The data will be keeped in process
        When(() => Random.Shared.Next(10) == 0, () => Value += "!");
    }

    void OnRun()
    {
        Console.Clear();
        Console.WriteLine(Value);
        Thread.Sleep(100);
    }
}
```

### Make complex applications with Bind expressions and components

```cs
using System;
using System.Text;
using System.Collections.Generic;

using Blindness.Core;

App.Start<LoginScreen>();

public interface LoginScreen : INode
{
    // Bindable properties
    Panel Panel { get; set; }
    TextBox Login { get; set; }
    TextBox Password { get; set; }
    TextBox Repeat { get; set; }
    string login { get; set; }
    string password { get; set; }
    string repeat { get; set; }
    bool registerPage { get; set; }
    int selectedField { get; set; }
    List<INode> children { get; set; }

    // Dependencies based on name
    void Deps(
        Panel Panel, 
        TextBox Login, 
        TextBox Password,
        TextBox Repeat
    );

    void OnLoad()
    {
        Panel.Width = 60;

        Login.Title = "login";
        Login.Size = 40;

        Password.Title = "password";
        Password.Text = "";
        Password.Size = 40;

        Repeat.Title = "repeat password";
        Repeat.Size = 40;

        // Bind expressions
        Bind(() => login == Login.Text);
        Bind(() => password == Password.Text);
        Bind(() => repeat == Repeat.Text);
        Bind(() => children == Panel.Children);
        Bind(() => Login.Selected == (selectedField == 0));
        Bind(() => Password.Selected == (selectedField == 1));
        Bind(() => Repeat.Selected == (selectedField == 2));
        Bind(() => Panel.Title == (registerPage ? "Register Page" : "Login Page"));

        registerPage = true;

        // ADd events
        When(
            () => registerPage,
            () => children = [ Login, Password, Repeat ]
        );

        When(
            () => !registerPage,
            () => children = [ Login, Password ]
        );

        On(
            () => 
                !registerPage ||
                password.Length > 5  &&
                password.Length < 50 &&
                password == repeat,
            r =>
                Password.Title = r ? 
                "password" :
                "password (has errors)"
        );
    }

    void OnRun()
    {
        Console.Clear();
        Panel.Run();

        var newChar = Console.ReadKey(true);
        if (newChar.Key == ConsoleKey.Tab)
        {
            selectedField = (selectedField, registerPage) switch
            {
                (2, true) => 0,
                (1, false) => 0,
                (var n, _) => n + 1
            };
            return;
        }

        if (newChar.Key == ConsoleKey.Backspace)
        {
            switch (selectedField)
            {
                case 0:
                    if (login.Length == 0)
                        break;
                    login = login[..^1];
                    break;

                case 1:
                    if (password.Length == 0)
                        break;
                    password = password[..^1];
                    break;

                case 2:
                    if (repeat.Length == 0)
                        break;
                    repeat = repeat[..^1];
                    break;
            }
            return;
        }

        if (newChar.Key == ConsoleKey.Enter)
        {
            registerPage = false;
            selectedField = 0;
            return;
        }

        switch (selectedField)
        {
            case 0:
                login += newChar.KeyChar;
                break;

            case 1:
                password += newChar.KeyChar;
                break;

            case 2:
                repeat += newChar.KeyChar;
                break;
        }
    }
}

public interface Panel : INode
{
    string Title { get; set; }
    int Width { get; set; }
    List<INode> Children { get; set; }

    void OnRun()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("─ ");
        sb.Append(Title);
        sb.Append(" ");
        sb.Append('─', Width - Title.Length - 2);
        Console.WriteLine(sb);

        foreach (var child in Children)
            child.Run();

        sb.Clear();
        sb.Append('─', Width);
        Console.WriteLine(sb);
    }
}

public interface TextBox : INode
{
    string Title { get; set; }
    string Text { get; set; }
    int Size { get; set; }
    bool Selected { get; set; }

    void OnRun()
    {
        StringBuilder sb = new StringBuilder();
        int size = 8 * Size / 10;
        Text ??= "";
        var text = Text.Length < size ? Text : 
            Text.Substring(
            Text.Length - size, size
        );

        if (Selected)
        {
            sb.Append("╔");
            sb.Append(Title);
            sb.Append('═', Size + 2 - Title.Length);
            sb.AppendLine("╗");

            sb.Append("║ ");
            sb.Append(text);
            sb.Append(' ', Size + 1 - text.Length);
            sb.AppendLine("║");

            sb.Append("╚");
            sb.Append('═', Size + 2);
            sb.AppendLine("╝");
        }
        else
        {
            sb.Append("┌");
            sb.Append(Title);
            sb.Append('─', Size + 2 - Title.Length);
            sb.AppendLine("┐");

            sb.Append("│ ");
            sb.Append(text);
            sb.Append(' ', Size + 1 - text.Length);
            sb.AppendLine("│");

            sb.Append("└");
            sb.Append('─', Size + 2);
            sb.AppendLine("┘");
        }

        Console.WriteLine(sb);
    }
}
```

### ...Or use only the resources that you like

```cs
using Blindness.Concurrency;
using Blindness.Injection;
using Blindness.Factory;
using Blindness.Bind;
using Blindness.Reload;
```

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
oddIndex.Value = lastOddIndex; // (Reverse calcule) index.Value = (oddIndex.Value - 1) / 2 = 2
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

### Customize the binding pipeline

```cs
using System;
using System.Linq.Expressions;

using Blindness.Bind;
using Blindness.Bind.Boxes;
using Blindness.Bind.Analyzers;

Binding.SetBehaviour(
    new DefaultLeftBindAnalyzer(), // left analyzer
    new MyBindAnalyzer(), // right analyzer
    new MyBindBehavior() // behaviour
)

public class MyConstantBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        var body = args.Body.RemoveTypeCast();
        if (body is not ConstantExpression constant)
            return BindingResult.Unsuccesfull;
        
        if (constant.value is null)
            throw new Exception();

        return BindingResult.Successful(
            Box.CreateConstant(constant.Value)
        );
    }
}

public class MyBindAnalyzer : IBindAnalyzer
{
    public BindChain BuildChain() => 
        BindChain.New()
        .Add(new BinaryOperationBindChainLink())
        .Add(new ExpressionBindChainLink())
        .Add(new CallBindChainLink())
        .Add(new MyConstantBindChainLink());
}

public class MyBindBehavior : IBindBehavior
{
    public void MakeBinding(BindingResult left, BindingResult right)
    {
        var leftReadonly = Box.IsReadOnly(left.MainBox);
        var rightReadonly = Box.IsReadOnly(right.MainBox);
        
        if (leftReadonly && rightReadonly)
            throw new ReadonlyBindingException();
        
        Box.SetInner(left.MainBox, right.MainBox);
    }
}
```

### Update your code with Blindness.Reload

```cs
using System.Threading;

using Blindness;
using Blindness.Reload;

var reloader = Reloader.GetDefault();
dynamic component = new MyComponent();

reloader.OnReload += assembly =>
{
    var newMyComponent = assembly.GetType("MyComponent");
    var constructor = newMyComponent.GetConstructor([]);
    var obj = constructor.Invoke([]);
    component = obj;
};

while (true)
{
    Console.Clear();
    component.Print();
    reloader.TryReload();
    Thread.Sleep(200);
}

public interface BaseComponent { void Print(); }
public class MyComponent : BaseComponent {
    public void Print()
        => Verbose.Success("Message...");
}
```

The app display 'Message...' on screen continuously. If the code is modified likes:

```cs
public class MyComponent : BaseComponent {
    public void Print()
        => Verbose.Success("Message!!");
}
```

Some time the code start to display 'Message!!' on screen without reestart app.

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
