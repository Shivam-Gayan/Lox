using Lox.Ast;
using Lox.Ast.Expressions;
using Lox.Ast.Statements;
using Lox.Runtime.ControlFlow;
using Lox.Runtime.Errors;
using Lox.Scanner;
using System.Net.Http.Headers;


namespace Lox.Runtime
{
    public class Interpreter : Expr.IVisitor<Object?>, Stmt.IVisitor<Object?>
    {
        public readonly Environment globals = new();
        private Environment environment;
        private readonly Dictionary<Expr, int> locals = [];
        private int loopDepth = 0;


        public Interpreter()
        {
            environment = globals;
            NativeFunctions.Register(globals);
        }
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach(Stmt statement in statements)
                {
                    Execute(statement);
                }
            } catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        //============================================
        //            Statement Visitors
        //============================================

        public object? VisitExpressionStmt(Expression stmt)
        {
            Evaluate(stmt.ExpressionValue);
            return null;
        }

        public object? VisitPrintStmt(Print stmt)
        {
            object value = Evaluate(stmt.ExpressionValue);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object? VisitVarStmt(Var stmt)
        {
            object? value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            environment.Define(stmt.Name.lexeme, value);
            return null;
        }

        public object? VisitBlockStmt(Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(environment));
            return null;
        }

        public object? VisitIfStmt(If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            } else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }

            return null;
        }

        public object? VisitWhileStmt(While stmt)
        {
            loopDepth++;

            try
            {
                while (IsTruthy(Evaluate(stmt.Condition)))
                {
                    try
                    {
                        Execute(stmt.Body);
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                    catch (BreakException)
                    {
                        break;
                    }
                }
            } finally
            {
                loopDepth--;
            }
            

            return null;
        }

        public object? VisitBreakStmt(Break stmt)
        {
            if (loopDepth == 0) throw new RuntimeError(stmt.Keyword, "Cannot use 'break' outside of a loop.");
            throw new BreakException();
        }

        public object? VisitContinueStmt(Continue stmt)
        {
            if (loopDepth == 0) throw new RuntimeError(stmt.Keyword, "Cannot use 'continue' outside of a loop.");
            throw new ContinueException();
        }

        public object? VisitFunctionStmt(Function stmt)
        {
            LoxFunction function = new(stmt, environment, false);
            environment.Define(stmt.Name.lexeme, function);
            return null;
        }

        public object? VisitReturnStmt(Return stmt)
        {
            object value = null;
            if (stmt.Value != null) value = Evaluate(stmt.Value);

            throw new ReturnException(value);
        }

        public object? VisitClassStmt(Class stmt)
        {
            object? superclass = null;

            if (stmt.Superclass != null)
            {
                superclass = Evaluate(stmt.Superclass);
                if(!(superclass is LoxClass))
                {
                    throw new RuntimeError(stmt.Superclass.Name,"Superclass must be a class.");
                }
            }

            environment.Define(stmt.Name.lexeme, null);

            if (stmt.Superclass != null)
            {
                environment = new Environment(environment);
                environment.Define("super", superclass);
            }

            Dictionary<string, LoxFunction> methods = [];
            foreach(Function method in stmt.Methods)
            {
                LoxFunction function = new(method, environment, method.Name.lexeme.Equals("init"));
                methods[method.Name.lexeme] = function;
            }

            LoxClass klass = new(stmt.Name.lexeme, (LoxClass)superclass, methods);

            if (superclass != null) 
            {
                environment = environment.enclosing;
            };

            environment.Assign(stmt.Name, klass);
            return null;
        }

        //============================================
        //            Expression Visitors             
        //============================================
        public object VisitAssignExpr(Assign expr)
        {
            object value = Evaluate(expr.Value);

            if(locals.TryGetValue(expr, out int val))
            {
                environment.AssignAt(val, expr.Name, value);
            } else
            {
                globals.Assign(expr.Name, value);
            }

            return value;
        }

        public object? VisitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.type)
            {
                // ==========================
                // Equality
                // ==========================
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);

                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);

                // ==========================
                // Arithmetic
                // ==========================
                case TokenType.MINUS:
                    return NumericBinary(expr.Operator, left, right, (a, b) => a - b);

                case TokenType.STAR:
                    return NumericBinary(expr.Operator, left, right, (a, b) => a * b);

                case TokenType.SLASH:
                    return SafeDivide(expr.Operator, left, right);

                case TokenType.PLUS:
                    return HandlePlus(expr.Operator, left, right);

                // ==========================
                // Comparisons (numbers only)
                // ==========================
                case TokenType.GREATER:
                    return NumericComparison(expr.Operator, left, right, (a, b) => a > b);

                case TokenType.GREATER_EQUAL:
                    return NumericComparison(expr.Operator, left, right, (a, b) => a >= b);

                case TokenType.LESS:
                    return NumericComparison(expr.Operator, left, right, (a, b) => a < b);

                case TokenType.LESS_EQUAL:
                    return NumericComparison(expr.Operator, left, right, (a, b) => a <= b);

                // ==========================
                // Bitwise (integers only)
                // ==========================
                case TokenType.BIT_AND:
                    return BitwiseBinary(expr.Operator, left, right, (a, b) => a & b);

                case TokenType.BIT_OR:
                    return BitwiseBinary(expr.Operator, left, right, (a, b) => a | b);

                case TokenType.BIT_XOR:
                    return BitwiseBinary(expr.Operator, left, right, (a, b) => a ^ b);

                case TokenType.SHIFT_LEFT:
                    return BitwiseBinary(expr.Operator, left, right, (a, b) => a << b);

                case TokenType.SHIFT_RIGHT:
                    return BitwiseBinary(expr.Operator, left, right, (a, b) => a >> b);
            }

            return null;
        }

        public object VisitCallExpr(Call expr)
        {
            object callee = Evaluate(expr.Callee);

            List<object> arguments = [];
            foreach (Expr argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (callee is not ILoxCallable)
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            }
            ILoxCallable function = (ILoxCallable)callee;

            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }


            return function.Call(this, arguments);
        }

        public object VisitGetExpr(Get expr)
        {
            object obj = Evaluate(expr.Object);

            if (obj is LoxInstance)
            {
                return ((LoxInstance)obj).Get(expr.Name);
            }

            throw new RuntimeError(expr.Name, "Only instances have properties.");
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object? VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.Operator.type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            } else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        public object VisitSetExpr(Set expr)
        {
            object obj = Evaluate(expr.Object);

            if(!(obj is LoxInstance))
            {
                throw new RuntimeError(expr.Name, "Only instances have fields.");

            }
            object value = Evaluate(expr.Value);
            ((LoxInstance)obj).Set(expr.Name, value);
            return value;
        }

        public object VisitSuperExpr(Super expr)
        {
            int distance = locals[expr];
            LoxClass superclass = (LoxClass)environment.GetAt(distance, "super");

            LoxInstance obj = (LoxInstance)environment.GetAt(distance - 1, "this");

            LoxFunction method = superclass.FindMethod(expr.Method.lexeme);

            if (method != null)
            {
                throw new RuntimeError(expr.Method, $"Undefined property '{expr.Method.lexeme}'.");
            }

            return method.Bind(obj);
        }

        public object VisitThisExpr(This expr)
        {
            return LookUpVariable(expr.Keyword, expr);
        }

        public object? VisitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Operator.type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);

                case TokenType.MINUS:
                    RequireNumber(expr.Operator, right);
                    return -(double)right;

                case TokenType.BIT_NOT:
                    RequireNumber(expr.Operator, right);
                    return ~(int)(double)right;
            }

            return null;
        }

        public object VisitVariableExpr(Variable expr)
        {
            return LookUpVariable(expr.Name, expr);
        }

        //=============================================
        //               Helper Methods
        //=============================================
        private object LookUpVariable(Token name, Expr expr)
        {
            if (locals.TryGetValue(expr, out int distance))
            {
                return environment.GetAt(distance, name.lexeme);
            } else
            {
                return globals.Get(name);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public void Resolve(Expr expr, int depth)
        {
            locals.Add(expr, depth);
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this.environment;

            try
            {
                this.environment = environment;

                foreach(Stmt statement in statements)
                {
                    Execute(statement);
                }
            } finally {
                this.environment = previous;
            }
        }

        private object NumericBinary(Token op, object left, object right, Func<double, double, double> operation)
        {
            RequireTwoNumbers(op, left, right);
            return operation((double)left, (double)right);
        }

        private object NumericComparison(Token op, object left, object right, Func<double, double, bool> comparison)
        {
            RequireTwoNumbers(op, left, right);
            return comparison((double)left, (double)right);
        }
        private object SafeDivide(Token op, object left, object right)
        {
            RequireTwoNumbers(op, left, right);

            double divisor = (double)right;
            if (divisor == 0)
                throw new RuntimeError(op, "Division by zero.");

            return (double)left / divisor;
        }

        private object HandlePlus(Token op, object left, object right)
        {
            if (left is double l && right is double r)
                return l + r;

            if (left is string ls && right is string rs)
                return ls + rs;

            throw new RuntimeError(op,
                "Operands must be two numbers or two strings.");
        }
        private object BitwiseBinary(Token op, object left, object right, Func<int, int, int> operation)
        {
            RequireTwoNumbers(op, left, right);

            double dl = (double)left;
            double dr = (double)right;

            if (dl % 1 != 0 || dr % 1 != 0)
                throw new RuntimeError(op, "Bitwise operands must be integers.");

            int a = (int)dl;
            int b = (int)dr;

            return operation(a, b);
        }

        private void RequireNumber(Token op, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void RequireTwoNumbers(Token op, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }


        public object Evaluate(Expr expr)
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

        public string? Stringify(object obj)
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
