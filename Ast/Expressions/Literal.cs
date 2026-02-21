using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Literal(object? value) : Expr
    {
        public readonly object? Value = value;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
}
