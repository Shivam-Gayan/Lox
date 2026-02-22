using Lox.Ast;
using Lox.Ast.Expressions;
using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Runtime
{
    public class Interpreter : Expr.IVisitor<Object>
    {


        public void Interpret(Expr expression)
        {
            try
            {
                object value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            } catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        //============================================
        //            Expression Visitors
        //============================================
        public object VisitAssignExpr(Assign expr)
        {
            throw new NotImplementedException();
        }

        public object VisitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.type)
            {
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);

                case TokenType.BIT_AND:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)(double)left & (int)(double)right;
                case TokenType.BIT_OR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)(double)left | (int)(double)right;
                case TokenType.BIT_XOR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)(double)left ^ (int)(double)right;

                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left is double l && right is double r)
                    {
                        return l + r;
                    }

                    if (left is string le && right is string ri)
                    {
                        return le + ri;
                    }

                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
            }

            return null;
        }

        public object VisitCallExpr(Call expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGetExpr(Get expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            throw new NotImplementedException();
        }

        public object VisitSetExpr(Set expr)
        {
            throw new NotImplementedException();
        }

        public object VisitSuperExpr(Super expr)
        {
            throw new NotImplementedException();
        }

        public object VisitThisExpr(This expr)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Operator.type)
            {
                case TokenType.BIT_NOT:
                    CheckNumberOperand(expr.Operator, right);
                    return ~(int)(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    return -(double)right;
            }

            return null;
        }

        public object VisitVariableExpr(Variable expr)
        {
            throw new NotImplementedException();
        }

        //=============================================
        //         Expression Helper Methods
        //=============================================

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool b) return b;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token opr, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(opr, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token opr, object left, object right)
        {
            if (left is double && right is double) return;

            throw new RuntimeError(opr, "Operands must be numbers.");
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is double d)
            {
                string text = d.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return obj.ToString();
        }
    }
}
