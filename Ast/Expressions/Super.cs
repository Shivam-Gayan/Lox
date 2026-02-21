using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Expressions
{
    public class Super(Token keyword, Token method) : Expr
    {
        public readonly Token Keyword = keyword;
        public readonly Token Method = method;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitSuperExpr(this);
        }
    }
}
