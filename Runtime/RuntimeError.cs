using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public class RuntimeError(Token token, string message) : ApplicationException(message)
    {
        public Token Token { get; } = token;
    }
}
