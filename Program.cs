using Lox;
using System;
class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: lox [script]");
            System.Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            // runFile(args[0]);
        }
        else
        {
            // runPrompt(); 

        }
    }
}