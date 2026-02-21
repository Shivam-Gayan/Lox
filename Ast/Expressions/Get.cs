using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Get(Expr obj, Token name) : Expr
    {
        public readonly Expr Object = obj;
        public readonly Token Name = name;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGetExpr(this);
        }
    }
}
