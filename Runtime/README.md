# Runtime Execution Engine & Semantic Analysis

Welcome to the **Runtime** module. If the `Scanner` provides the vocabulary and the `Parser` provides the grammar, the `Runtime` is where the Lox language actually comes to life. 

This directory contains the **Tree-Walk Interpreter**, the **Semantic Analyzer** (`Resolver`), the memory management system (`Environment`), and the complete **Object-Oriented Data Model**.

## Architecture: The Tree-Walk Interpreter

At its core, Lox is a **Tree-Walk Interpreter**. The `Interpreter.cs` class implements the Visitor Pattern (`Expr.IVisitor` and `Stmt.IVisitor`) to perform a post-order traversal of the Abstract Syntax Tree (AST). 

* **Dynamic Typing:** Variables in Lox do not have static types. Under the hood, the interpreter evaluates everything down to a native C# `object`. Type checking (e.g., ensuring you don't subtract a string from a boolean) happens dynamically at runtime right before the operation is executed.
* **Truthiness:** Lox strictly defines `nil` and `false` as falsey. Everything else (including `0` and empty strings `""`) is truthy.

## Semantic Analysis: The `Resolver`

Before the interpreter executes a single line of code, the AST makes a pitstop at the `Resolver`. 

Because Lox has **First-Class Functions** and **Closures**, dynamically looking up variables by their string name at runtime can lead to severe scoping bugs (specifically, an inner function might accidentally grab a newly declared variable from a higher scope instead of the one it originally captured).

The `Resolver` performs a **Static Semantic Analysis Pass**:
1. **Scope Simulation:** It walks the AST and simulates block scopes using a stack of dictionaries.
2. **Lexical Binding:** When it finds a variable usage, it looks up the stack and calculates exactly how many "hops" up the environment chain it takes to find the declaration.
3. **Caching:** It caches this "hop distance" in the `Interpreter`. 
4. **Static Checks:** The resolver also performs critical validation that can't be done in the parser, ensuring that users don't use `break` outside a loop, `return` outside a function, or `this`/`super` outside a class.

When the interpreter runs, it uses these exact hop distances for O(1) variable lookups, ensuring **Static Lexical Scoping** works perfectly.

## Memory & State: The `Environment`

The `Environment` class is the heart of Lox's memory management.
* It is a wrapper around a C# `Dictionary<string, object>` that maps variable names to their evaluated C# values.
* **Environment Chains:** Every time a block `{ ... }` or a function body is entered, a new `Environment` is created with a reference to its `enclosing` (parent) environment. This forms a linked list representing the current lexical scope chain.

## Control Flow as Exceptions

In languages like C#, `return`, `break`, and `continue` are native language constructs. Because we are building an interpreter *on top* of C#, we have to implement these jumps manually.

We achieve this via **Call Stack Unwinding using Exceptions**. 
Look inside the `/ControlFlow` folder. When the interpreter evaluates a `return` statement, it throws a custom `ReturnException` containing the return value. The C# runtime immediately unwinds the call stack, jumping out of all nested loops and blocks, until it is caught by the `LoxFunction`'s execution method, which extracts the value and returns it to the caller. We use the exact same architecture for `BreakException` and `ContinueException` inside `while` loops!

## The Object Model (OOP)

This directory defines Lox's entire runtime type system for Object-Oriented Programming:

* **`LoxClass`:** Represents a declared class. It acts as a factory/constructor. Because it implements `ILoxCallable`, the interpreter can "call" a class (e.g., `Person()`) to instantiate it. It also holds a dictionary of the class's methods and a reference to its `superclass` for inheritance.
* **`LoxInstance`:** The actual instantiated object in memory. It maintains a dictionary of its state (fields/properties). When you look up a property (e.g., `jane.name`), it checks its internal fields first. If it's not a field, it asks its `LoxClass` if it's a method.
* **`LoxFunction`:** Represents a function or a method. Crucially, it holds a reference to the `Environment` that was active *when the function was declared*. This is what creates a **Closure**, allowing functions to remember variables that have long since fallen out of the global scope.
* **Method Binding:** When an instance accesses a method, `LoxFunction.Bind()` is called. This dynamically creates a new, invisible `Environment` scope containing the `this` keyword, binds it to the current `LoxInstance`, and wraps the function inside it.

## Foreign Function Interface (FFI)

The `/NativeFunctions.cs` and `ILoxCallable` interface allow us to inject raw C# execution into the Lox environment. For example, `NativeClock` is injected into the global environment on startup, allowing Lox scripts to call `clock()` to get the current system time in seconds.