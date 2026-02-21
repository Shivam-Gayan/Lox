using Lox.Ast;
using Lox.Ast.Expressions;
using Lox.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Parsing
{
    public class Parser(List<Token> token)
    {
        private readonly List<Token> tokens = token;
        private int current = 0;

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token operatorToken = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, operatorToken, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token operatorToken = Previous();
                Expr right = Term();
                expr = new Binary(expr, operatorToken, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while(Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token operatorToken = Previous();
                Expr right = Factor();
                expr = new Binary(expr, operatorToken, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();

            while(Match(TokenType.SLASH, TokenType.STAR))
            {
                Token operatorToken = Previous();
                Expr right = Unary();
                expr = new Binary(expr, operatorToken, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token operatorToken = Previous();
                Expr right = Unary();
                return new Unary(operatorToken, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().literal);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }
        }

        //=============================================
        //               Helper Methods
        //=============================================

        private bool Match(params TokenType[] types)
        {
            foreach(TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;

            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

    }
}
