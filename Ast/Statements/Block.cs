using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Ast.Statements
{
    public class Block(List<Stmt> statements) : Stmt
    {
        public List<Stmt> Statements { get; } = statements;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }
}
