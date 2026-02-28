using Lox.Scanner;
using Lox.Ast.Expressions;


namespace Lox.Ast.Statements
{
    public class Class(Token name, Variable superclass, List<Function> methods) : Stmt
    {
        public Token Name { get; } = name;
        public Variable Superclass { get; } = superclass;
        public List<Function> Methods { get; } = methods;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitClassStmt(this);
        }
    }
}
