using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    public class Lox
    {
        static bool hadError = false;

        public static void RunFile(String Path)
        {
            if (File.Exists(Path))
            {
                byte[] bytes = File.ReadAllBytes(Path);
                Run(Encoding.UTF8.GetString(bytes));

                if (hadError) System.Environment.Exit(65);

            } else
            {
                Console.WriteLine("File doesn't Exist \n");
                throw new FileNotFoundException();
            }
        }
        public static void RunPrompt()
        {
            for(;;)
            {
                Console.WriteLine("> ");

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

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
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
        private static void Report(int line,string where,string message)
        {
            Console.Error.WriteLine("[Line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

    }
}
