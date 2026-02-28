using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Return(Token keyword, Expr value) : Stmt
    {
        public Token Keyword { get; } = keyword;
        public Expr Value { get; } = value;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }
    }
}
