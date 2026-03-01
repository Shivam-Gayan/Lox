using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime.ControlFlow
{
    public class ReturnException(object? value) : Exception
    {
        public object? Value { get; } = value;
    }
}
