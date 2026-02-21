using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Scanner
{
    public class Token(TokenType type, string lexeme, object? literal, int line)
    {
        public readonly TokenType type = type;
        public readonly String lexeme = lexeme;
        public readonly Object? literal = literal;
        public readonly int line = line;


        public override String ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}
