using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Scanner
{
    public enum TokenType
    {
        // Single Character Tokens
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH,STAR,

        // One Or Two character Tokens
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Bitwise 
        BIT_AND,
        BIT_OR,
        BIT_XOR,
        BIT_NOT,
        SHIFT_LEFT,
        SHIFT_RIGHT,

        // Literals
        IDENTIFIER, STRING, NUMBER,

        // Keywords
        AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR, 
        PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

        EOF

    }
}
