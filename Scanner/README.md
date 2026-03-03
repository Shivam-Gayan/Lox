# Scanner (Lexical Analysis)

Welcome to the **Lexical Analysis** module of the Lox interpreter. Before the parser can understand the grammar of the code, the raw string of characters must be grouped into meaningful "words." This process is known as **Scanning**, **Lexing**, or **Tokenization**.

This directory contains the `Scanner`, which is the very first phase of the interpreter pipeline.

## Architecture: Hand-Coded Lexer

While many compilers use automated tools (like Lex or Flex) to generate scanners from regular expressions, this project features a **Hand-Coded Lexer**. Hand-coded scanners are incredibly fast, easier to debug, and provide much better error reporting.

### Key Concepts Used:
* **The Maximal Munch Principle (Greedy Lexing):** When the scanner reads a character, it always tries to match the longest possible token. For example, if it reads `<`, it doesn't immediately output a `LESS` token. It looks ahead to see if the next character is `=` (to output `LESS_EQUAL`) or `<` (to output `SHIFT_LEFT`).
* **Lookahead (k=2):** The scanner uses `Peek()` and `PeekNext()` to look one or two characters ahead in the source code without consuming them. This is crucial for distinguishing between `/` (division), `//` (single-line comment), and `/*` (multi-line block comment).
* **Reserved Words vs. Identifiers:** When the scanner encounters a sequence of letters, it first assumes it's a user-defined variable (an `IDENTIFIER`). Before emitting the token, it checks a hash map of reserved keywords (like `if`, `class`, `while`). If there's a match, it emits the specific keyword token instead.

## Core Components

### 1. `Scanner.cs` (The Engine)
The main state machine that iterates through the source code character by character. 
* It maintains pointers (`start` and `current`) to slice the source string into lexemes.
* It safely ignores meaningless whitespace, tabs, and carriage returns.
* It tracks line numbers so that syntax errors and runtime errors can accurately point the user to the exact location of the bug.
* **Extended Features:** Unlike standard Lox, this scanner has been upgraded to recognize C-style block comments (`/* ... */`) and bitwise operators (`&`, `|`, `^`, `~`, `<<`, `>>`).

### 2. `Token.cs` (The Data Transfer Object)
A simple, immutable DTO that represents a single meaningful chunk of code. A `Token` bundles together:
* **`type`**: What kind of token it is (e.g., `NUMBER`, `PLUS`, `IF`).
* **`lexeme`**: The exact string of characters from the source code (e.g., `"123.45"`, `"+"`, `"if"`).
* **`literal`**: The parsed C# runtime value. For a string token, this is the actual C# string with quotes removed. For a number token, this is a C# `double`.
* **`line`**: The line number where the token appeared.

### 3. `TokenType.cs` (The Vocabulary)
An `enum` defining every single valid symbol and keyword in the Lox language. It is categorized into:
* **Single-character tokens:** `(`, `)`, `{`, `}`, `,`, `.`, `-`, `+`, `;`, `/`, `*`
* **One or two-character tokens:** `!`, `!=`, `=`, `==`, `>`, `>=`, `<`, `<=`
* **Bitwise tokens:** `&`, `|`, `^`, `~`, `<<`, `>>`
* **Literals:** `IDENTIFIER`, `STRING`, `NUMBER`
* **Keywords:** `and`, `class`, `else`, `false`, `fun`, `for`, `if`, `nil`, `or`, `print`, `return`, `super`, `this`, `true`, `var`, `while`, `break`, `continue`
* **`EOF`**: A special "End of File" token that cleanly terminates the parsing phase.

## The Pipeline Handoff
Once the `Scanner` finishes its single pass over the source string, it returns a flat `List<Token>`. This list is immediately handed off to the `Parser` (in the `/Parsing` directory) to build the Abstract Syntax Tree.