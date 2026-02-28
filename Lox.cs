using Lox.Ast;
using Lox.Ast.Statements;
using Lox.Parsing;
using Lox.Runtime;
using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    public class Lox
    {
        static bool hadError = false;
        static bool hadRuntimeError = false;
        private static bool replMode = false;
        private static readonly Interpreter interpreter = new();
        public static void RunFile(String Path)
        {
            if (File.Exists(Path))
            {
                byte[] bytes = File.ReadAllBytes(Path);
                Run(Encoding.UTF8.GetString(bytes));

                if (hadError) System.Environment.Exit(65);
                if (hadRuntimeError) System.Environment.Exit(70);

            } else
            {
                Console.WriteLine("File doesn't Exist \n");
                throw new FileNotFoundException();
            }
        }
        public static void RunPrompt()
        {
            replMode = true;

            for (;;)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (input == null) break;

                Run(input);
                hadError = false;
            }
        }
        public static void Run(string source)
        {
            Scan scanner = new(source);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new(tokens);
            List<Stmt> statements = parser.Parse();

            if (hadError) return;

            if (replMode &&
                statements.Count == 1 &&
                statements[0] is Expression exprStmt)
            {
                var value = interpreter.Evaluate(exprStmt.ExpressionValue);
                Console.WriteLine(interpreter.Stringify(value));
            }
            else
            {
                interpreter.Interpret(statements);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }
        public static void Error(Token token, string messsage)
        {
            if (token.type == TokenType.EOF)
            {
                Report(token.line, " at end", messsage);
            } else
            {
                Report(token.line, " at '" + token.lexeme + "'", messsage);
            }
        }

        public static void RuntimeError(RuntimeError error) 
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.line}]");
            hadRuntimeError = true;
        }
        private static void Report(int line,string where,string message)
        {
            Console.Error.WriteLine("[Line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

    }
}
