using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.Scanner
{
    public class Scanner(string source)
    {
        private readonly String source = source;
        private readonly List<Token> tokens = [];
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        // Private Helper Methods

        private void ScanToken()
        {
            char c = Advance();
            switch(c)
            {

                // Single Character Lexems
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;

                // Bitwise Single Characters
                case '&': AddToken(TokenType.BIT_AND); break;
                case '|': AddToken(TokenType.BIT_OR); break;
                case '^': AddToken(TokenType.BIT_XOR); break;
                case '~': AddToken(TokenType.BIT_NOT); break;

                // One Or Two character Tokens
                case '!': // !=
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=': // ==
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<': // <= or <<
                    if (Match('<'))
                    {
                        AddToken(TokenType.SHIFT_LEFT);
                    } else
                    {
                        AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); 
                    }
                    break;
                case '>': // >> or >=
                    if(Match('>'))
                    {
                        AddToken(TokenType.SHIFT_RIGHT);
                    } else
                    {
                        AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    }
                    break;

                // Special Cases
                case '/':
                    if (Match('/'))
                    {
                        // comment goes until the end of line
                        while(Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    } else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;
                case '"':

                    break;

                default:
                    Lox.Error(line, "Unexpected Character.");
                    break;
            }
        }

        private char Advance() { return source[current++]; }

        private void AddToken(TokenType type) { AddToken(type, null); }

        private void AddToken(TokenType type, Object? literal)
        {
            string text = source[start..current];
            tokens.Add(new Token(type, text, literal, line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }
        private bool IsAtEnd() { return current >= source.Length; }

    }
}
