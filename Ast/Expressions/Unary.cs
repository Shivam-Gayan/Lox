using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Unary(Token operatorToken, Expr right) : Expr
    {
        public readonly Token Operator = operatorToken;
        public readonly Expr Right = right;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
