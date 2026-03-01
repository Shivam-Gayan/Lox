using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public interface ILoxCallable
    {
        public int Arity();
        public object Call(Interpreter interpreter, List<object> arguments);
    }
}
