using Lox.Ast.Statements;
using Lox.Runtime.ControlFlow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public class LoxFunction(Function declaration, Environment closure) : ILoxCallable
    {
        private readonly Function declaration = declaration;
        private readonly Environment closure = closure;

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
                return returnValue.Value;
            }
            return null;
        }

        public override string ToString()
        {
            return $"<fn {declaration.Name.lexeme}>";
        }
    }
}
