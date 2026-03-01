using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Break(Token keyword) : Stmt
    {
        public Token Keyword { get; } = keyword;
        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBreakStmt(this);
        }
    }
}
