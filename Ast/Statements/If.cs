using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class If(Expr condition, Stmt thenBranch, Stmt elseBranch) : Stmt
    {
        public Expr Condition { get; } = condition;
        public Stmt ThenBranch { get; } = thenBranch;
        public Stmt ElseBranch { get; } = elseBranch;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }
}
