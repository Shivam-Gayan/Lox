using Lox.Ast.Statements;
using Lox.Runtime.ControlFlow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public class LoxFunction(Function declaration, Environment closure, bool isinitializer) : ILoxCallable
    {
        private readonly Function declaration = declaration;
        private readonly Environment closure = closure;
        private readonly bool isInitializer = isinitializer;
        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object? Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new(closure);

            for (int i = 0; i < declaration.Parameters.Count; i++)
            {
                environment.Define(declaration.Parameters[i].lexeme, arguments[i]);
            }
            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            } catch (ReturnException returnValue)
            {
                if (isInitializer) return closure.GetAt(0, "this");

                return returnValue.Value;
            }

            if (isInitializer) return closure.GetAt(0, "this");

            return null;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new(closure);
            environment.Define("this", instance);

            return new LoxFunction(declaration, environment, isInitializer);
        }

        public override string ToString()
        {
            return $"<fn {declaration.Name.lexeme}>";
        }
    }
}
