using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Function(Token name, List<Token> parameters, List<Stmt> body) : Stmt
    {
        public Token Name { get; } = name;
        public List<Token> Parameters { get; } = parameters;
        public List<Stmt> Body { get; } = body;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }
    }
}
