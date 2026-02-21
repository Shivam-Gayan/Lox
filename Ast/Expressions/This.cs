using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class This(Token keyword) : Expr
    {
        public readonly Token Keyword = keyword;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitThisExpr(this);
        }
    }
}
