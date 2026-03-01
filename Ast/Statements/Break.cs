using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Break : Stmt
    {
        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBreakStmt(this);
        }
    }
}
