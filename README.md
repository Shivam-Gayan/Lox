# Lox Interpreter in C#

Welcome to the **C# Lox Interpreter**, a fully-featured, Turing-complete programming language implementation based on Bob Nystrom's *Crafting Interpreters*. This project is a robust **Tree-Walk Interpreter** built with **.NET 10**, featuring a custom hand-coded lexer, a recursive descent parser, and a dynamic runtime environment.

## Features
* **Recursive Descent Parsing:** A top-down, predictive parser that translates tokens into an Abstract Syntax Tree (AST) without external parser generators.
* **Visitor Pattern:** Decouples operations (like interpreting and resolving) from the AST nodes, adhering to the Open/Closed Principle for clean, maintainable object-oriented design.
* **Lexical Scoping & Closures:** Implements static scoping with a dedicated semantic analysis pass (`Resolver`), allowing functions to capture and retain their surrounding environment state.
* **Dynamic Typing:** Evaluates types at runtime, supporting numbers, strings, booleans, and `nil`.
* **Object-Oriented Programming (OOP):** First-class support for classes, instances, methods, inheritance (`<`), and `super` calls.
* **First-Class Functions:** Functions can be passed as arguments, returned from other functions, and bound to instances as methods.
* **Interactive REPL:** A Read-Eval-Print Loop for real-time code execution and testing.
* **Advanced Control Flow:** Support for `if`, `for`, `while`, as well as advanced loop controls like `break` and `continue`.
* **Bitwise Operations:** Extended Lox syntax to support C-style bitwise operators (`&`, `|`, `^`, `<<`, `>>`, `~`).

## Architecture Overview
The execution pipeline follows a classic interpreter architecture:
1. **[Scanner (Lexical Analysis)](./Scanner/README.md):** Converts raw source code strings into a sequence of meaningful `Token` objects.
2. **[Parser (Syntax Analysis)](./Parsing/README.md):** Consumes tokens and constructs an Abstract Syntax Tree (AST) using recursive descent.
3. **[AST (Abstract Syntax Tree)](./Ast/README.md):** A static analysis pass that resolves variable bindings to ensure lexical scope is preserved before execution.
4. **[Runtime Engine (Evaluation & State)](./Runtime/README.md):** Walks the AST using the Visitor pattern, managing state, evaluating expressions, and executing statements in real-time.

## Getting Started
You can run Lox either by downloading the pre-compiled executable or by building it from source.

### Option 1: Standalone Executable (Recommended)
You do not need the .NET SDK to run the pre-built version. 
1. Download the latest `lox.exe` from the **Releases** tab.
2. Open your terminal or command prompt.

**Run the interactive REPL:**
```bash
./lox.exe
```
**Run a Lox script:**
```bash
./lox.exe path/to/script.lox
```

### Option 2: Build from Source
If you want to modify the interpreter or explore the code, you will need the .NET 10 SDK installed.

Clone the repository.

Navigate to the root directory.

**Run the interactive REPL:**
```bash
dotnet run
```
**Run a Lox script:**
```bash
dotnet run -- path/to/script.lox
```
