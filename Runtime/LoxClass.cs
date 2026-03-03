using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public class LoxClass(string name, Dictionary<string, LoxFunction> methods) : ILoxCallable
    {
        public readonly string name = name;
        private readonly Dictionary<string, LoxFunction> methods = methods;

        public int Arity()
        {
            LoxFunction initializer = FindMethod("init");
            if (initializer != null) return 0;

            return initializer.Arity();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new(this);
            LoxFunction initializer = FindMethod("init");
            if (initializer != null) initializer.Bind(instance).Call(interpreter, arguments);
            return instance;
        }

        public LoxFunction? FindMethod(string name)
        {
            if (methods.TryGetValue(name, out LoxFunction? value))
            {
                return value;
            }

            return null;
        }

        public override string ToString()
        {
            return name;
        }

    }
}
