using Lox.Runtime.Errors;
using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public class LoxInstance(LoxClass klass)
    {
        private LoxClass klass = klass;
        private readonly Dictionary<string, object> fields = [];

        public object Get(Token name)
        {
            if (fields.TryGetValue(name.lexeme, out object? value))
            {
                return value;
            };

            LoxFunction method = klass.FindMethod(name.lexeme);
            if (method != null) return method.Bind(this);

            throw new RuntimeError(name, $"Undefined property '{name.lexeme}'.");
        }

        public void Set(Token name, object value)
        {
            fields[name.lexeme] = value;
        }
        public override string ToString()
        {
            return $"{klass.name} instance";
        }
    }
}
