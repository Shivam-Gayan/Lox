using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public static class NativeFunctions
    {
        public static void Register(Environment globals)
        {
            globals.Define("clock", new NativeClock());
        }
    }
}
