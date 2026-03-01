using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Continue : Stmt
    {
        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitContinueStmt(this);
        }
    }
}
