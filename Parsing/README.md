# Parsing (Syntax Analysis)

Welcome to the **Syntax Analysis** module of the Lox interpreter. This directory houses the `Parser`, which is responsible for taking the linear sequence of `Token` objects produced by the Scanner and validating them against the Lox language grammar, ultimately constructing the **Abstract Syntax Tree (AST)**.

## Architecture: Top-Down Recursive Descent

This implementation utilizes a **Recursive Descent Parser**. It is a **Top-Down, LL(1) Predictive Parser**.
* **LL(1)**: It parses the input from **L**eft to right, constructs a **L**eftmost derivation of the syntax tree, and looks ahead exactly **1** token to predict which grammar rule to apply next.
* **Recursive Descent**: Every grammar rule maps directly to a C# method. Non-terminals in the grammar call other methods recursively, naturally building the tree structure via the C# call stack.

## The Lox Context-Free Grammar (EBNF)
The parser is a direct implementation of the following Context-Free Grammar, defined in Extended Backus-Naur Form (EBNF). 

### Declarations & Statements
```ebnf
program        → declaration* EOF ;
declaration    → classDecl | funDecl | varDecl | statement ;
classDecl      → "class" IDENTIFIER ( "<" IDENTIFIER )? "{" function* "}" ;
funDecl        → "fun" function ;
varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
statement      → exprStmt | forStmt | ifStmt | printStmt | returnStmt | whileStmt | block | breakStmt | continueStmt ;
exprStmt       → expression ";" ;
forStmt        → "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;
ifStmt         → "if" "(" expression ")" statement ( "else" statement )? ;
printStmt      → "print" expression ";" ;
returnStmt     → "return" expression? ";" ;
whileStmt      → "while" "(" expression ")" statement ;
breakStmt      → "break" ";" ;
continueStmt   → "continue" ";" ;
block          → "{" declaration* "}" ;
function       → IDENTIFIER "(" parameters? ")" block ;
parameters     → IDENTIFIER ( "," IDENTIFIER )* ;
```

## Expressions & Precedence
The expression grammar is stratified to automatically enforce operator precedence. Rules lower in this list have tighter binding (higher precedence).
```ebnf
expression     → assignment ;
assignment     → ( call "." )? IDENTIFIER "=" assignment | logic_or ;
logic_or       → logic_and ( "or" logic_and )* ;
logic_and      → equality ( "and" equality )* ;
equality       → comparison ( ( "!=" | "==" ) comparison )* ;
comparison     → shift ( ( ">" | ">=" | "<" | "<=" ) shift )* ;
shift          → bitwise_or ( ( "<<" | ">>" ) bitwise_or )* ;
bitwise_or     → bitwise_xor ( "|" bitwise_xor )* ;
bitwise_xor    → bitwise_and ( "^" bitwise_and )* ;
bitwise_and    → term ( "&" term )* ;
term           → factor ( ( "-" | "+" ) factor )* ;
factor         → unary ( ( "/" | "*" ) unary )* ;
unary          → ( "!" | "-" | "~" ) unary | call ;
call           → primary ( "(" arguments? ")" | "." IDENTIFIER )* ;
arguments      → expression ( "," expression )* ;
primary        → NUMBER | STRING | "true" | "false" | "nil" | "this" 
               | "super" "." IDENTIFIER | IDENTIFIER | "(" expression ")" ;
```

## Operator Precedence Table

| Precedence | Operator | Type | Associativity | Method in `Parser.cs` |
| :--- | :--- | :--- | :--- | :--- |
| **1** (Lowest) | `=` | Assignment | Right | `Assignment()` |
| **2** | `or` | Logical OR | Left | `Or()` |
| **3** | `and` | Logical AND | Left | `And()` |
| **4** | `==`, `!=` | Equality | Left | `Equality()` |
| **5** | `<`, `<=`, `>`, `>=` | Comparison | Left | `Comparison()` |
| **6** | `<<`, `>>` | Bitwise Shift | Left | `Shift()` |
| **7** | `\|` | Bitwise OR | Left | `BitwiseOr()` |
| **8** | `^` | Bitwise XOR | Left | `BitwiseXor()` |
| **9** | `&` | Bitwise AND | Left | `BitwiseAnd()` |
| **10** | `+`, `-` | Term (Add/Sub) | Left | `Term()` |
| **11** | `*`, `/` | Factor (Mult/Div) | Left | `Factor()` |
| **12** | `!`, `-`, `~` | Unary | Right | `Unary()` |
| **13** | `()`, `.` | Call, Property | Left | `Call()` |
| **14** (Highest)| Literals, `()` | Primary | N/A | `Primary()` |


## Error Handling: Panic-Mode Recovery
Compilers shouldn't crash on the first syntax error, nor should they emit a cascade of false-positive errors caused by a single missing semicolon.

This parser implements Panic-Mode Error Recovery. When a syntax error occurs, the parser enters panic mode and calls Synchronize(). This method discards tokens until it finds a statement boundary (like a ; or a keyword like class, fun, for, if). This aligns the parser's state back with the source code, allowing it to continue parsing and catch subsequent, genuine errors in the same pass.

## Desugaring (Syntactic Sugar)
Notice that there is no For.cs AST node in the codebase. The for loop in Lox is pure syntactic sugar.
During parsing, ForStatement() translates the for loop directly into a primitive While node wrapped in a Block node alongside its initializer and incrementer. This process, known as desugaring, keeps the core AST and the Interpreter small and clean by offloading the complexity to the parser.