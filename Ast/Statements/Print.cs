using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Print(Expr expression) : Stmt
    {
        public Expr ExpressionValue { get; } = expression;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

}
