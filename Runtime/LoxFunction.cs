using Lox.Ast.Statements;
using Lox.Runtime.ControlFlow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public class LoxFunction(Function declaration) : ILoxCallable
    {
        private readonly Function declaration = declaration;

        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object? Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new(interpreter.globals);

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
