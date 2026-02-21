using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Assign(Token name, Expr value) : Expr
    {
        public readonly Token Name = name;
        public readonly Expr Value = value;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }
}
