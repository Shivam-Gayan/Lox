using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class While(Expr condition, Stmt body) : Stmt
    {
        public Expr Condition { get; } = condition;
        public Stmt Body { get; } = body;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }
}
