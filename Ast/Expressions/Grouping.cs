using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Grouping(Expr expression) : Expr
    {
        public readonly Expr Expression = expression;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
}
