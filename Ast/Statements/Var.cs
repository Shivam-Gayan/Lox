using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Var(Token name, Expr initializer) : Stmt
    {
        public Token Name { get; } = name;
        public Expr Initializer { get; } = initializer;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }
}
