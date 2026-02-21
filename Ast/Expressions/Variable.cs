using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Variable(Token name) : Expr
    {
        public readonly Token Name = name;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }
}
