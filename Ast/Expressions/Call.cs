using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Call(Expr callee, Token paren, List<Expr> arguments) : Expr
    {
        public readonly Expr Callee = callee;
        public readonly Token Paren = paren;
        public readonly List<Expr> Arguments = arguments;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitCallExpr(this);
        }
    }
}
