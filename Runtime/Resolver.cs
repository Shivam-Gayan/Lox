using Lox.Ast;
using Lox.Ast.Expressions;
using Lox.Ast.Statements;
using Lox.Scanner;
using System.Runtime.InteropServices;



namespace Lox.Runtime
{
    public enum FunctionType
    {
        NONE,
        FUNCTION,
        METHOD,
        INITIALIZER
    }

    public enum ClassType
    {
        NONE,
        CLASS
    }
    public class Resolver(Interpreter interpreter) : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        private readonly Interpreter interpreter = interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new();
        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;
        public object? VisitBlockStmt(Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();

            return null;
        }

        public object? VisitVarStmt(Var stmt)
        {
            Declare(stmt.Name);

            if (stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }

            Define(stmt.Name);
            return null;
        }

        public object? VisitVariableExpr(Variable expr)
        {
            if (!(scopes.Count == 0) && scopes.Peek()[expr.Name.lexeme] == false)
            {
                Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object? VisitAssignExpr(Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);

            return null;
        }

        public object? VisitFunctionStmt(Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt, FunctionType.FUNCTION);

            return null;
        }

        public object? VisitExpressionStmt(Expression stmt)
        {
            Resolve(stmt.ExpressionValue);
            return null;
        }

        public object? VisitIfStmt(If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);

            return null;
        }

        public object?  VisitPrintStmt(Print stmt)
        {
            Resolve(stmt.ExpressionValue);
            return null;
        }

        public object? VisitReturnStmt(Return stmt)
        {

            if(currentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.Keyword, "Can't return from top-level code.");
            }

            if (stmt.Value != null)
            {
                if (currentFunction == FunctionType.INITIALIZER)
                {
                    Lox.Error(stmt.Keyword, "Can't return a value from an initializer.");
                }
                Resolve(stmt.Value);
            }

            return null;
        }

        public object? VisitWhileStmt(While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);

            return null;
        }

        public object? VisitBinaryExpr(Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return null;
        }

        public object? VisitCallExpr(Call expr)
        {
            Resolve(expr.Callee);

            foreach(Expr argument in expr.Arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public object? VisitGroupingExpr(Grouping expr)
        {
            Resolve(expr.Expression);

            return null;
        }

        public object? VisitLiteralExpr(Literal expr)
        {
            return null;
        }

        public object? VisitLogicalExpr(Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return null;
        }

        public object? VisitUnaryExpr(Unary expr)
        {
            Resolve(expr.Right);

            return null;
        }
        
        //======================================
        //            Helper Methods
        //======================================

        private void ResolveFunction(Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;
            BeginScope();

            foreach(Token parameter in function.Parameters)
            {
                Declare(parameter);
                Define(parameter);
            }
            Resolve(function.Body);
            EndScope();

            currentFunction = enclosingFunction;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme))
                {
                    interpreter.Resolve(expr, scopes.Count - 1 - i);
                    return;
                }
            }
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.lexeme] = true;
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;

            Dictionary<string, bool> scope = scopes.Peek();

            if(scope.ContainsKey(name.lexeme))
            {
                Lox.Error(name, "Already a variable with this name in this scope.");
            }

            scope[name.lexeme] = false;
        }
        private void BeginScope()
        {
            scopes.Push([]);
        }

        private void EndScope()
        {
            scopes.Pop();
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach(Stmt statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        public object? VisitGetExpr(Get expr)
        {
            Resolve(expr.Object);
            return null;
        }

        public object? VisitSetExpr(Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Object);
            return null;
        }

        public object? VisitSuperExpr(Super expr)
        {
            throw new NotImplementedException();
        }

        public object? VisitThisExpr(This expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.Error(expr.Keyword, "Can't use 'this' outside of a class.");

                return null;
            }

            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        public object? VisitClassStmt(Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;

            Declare(stmt.Name);
            Define(stmt.Name);

            BeginScope();
            scopes.Peek().Add("this", true);

            foreach(Function method in stmt.Methods)
            {
                FunctionType declaration = FunctionType.METHOD;

                if (method.Name.lexeme.Equals("init")) declaration = FunctionType.INITIALIZER;

                ResolveFunction(method, declaration);
            }

            EndScope();

            currentClass = enclosingClass;

            return null;
        }


        public object? VisitBreakStmt(Break stmt)
        {
            throw new NotImplementedException();
        }

        public object? VisitContinueStmt(Continue stmt)
        {
            throw new NotImplementedException();
        }
    }
}
