using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Scanner
{
    public class Token(TokenType type, string lexeme, object? literal, int line)
    {
        readonly TokenType type = type;
        readonly String lexeme = lexeme;
        readonly Object? literal = literal;
        readonly int line = line;


        public override String ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}
