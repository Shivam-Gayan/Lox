using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Lox.Scanner
{
    public class Scan(string source)
    {
        private readonly String source = source;
        private readonly List<Token> tokens = [];
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private readonly Dictionary<String, TokenType> keywords = new()
        {
            { "and" , TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.ELSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };
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
                case '!': // != or !
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=': // == or =
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
                    } 
                    else if (Match('*')) 
                    {
                        while (!(Peek() == '*' && PeekNext() == '/') && !IsAtEnd())
                        {
                            if (Peek() == '\n') line++;
                            Advance();
                        }

                        if (IsAtEnd())
                        {
                            Lox.Error(line, "Unterminated Comment");
                            return;
                        }

                        Advance(); // *
                        Advance(); // /
                    }
                    else
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
                    String();
                    break;
                

                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(line, "Unexpected Character.");
                    }
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            String text = source[start..current];

            TokenType type = keywords.TryGetValue(text, out TokenType keywordType) ? keywordType : TokenType.IDENTIFIER;

            AddToken(type);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_');
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Check for fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume .
                Advance();
                
                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER,double.Parse(source[start..current]));

        }


        private void String()
        {
            while(Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if(IsAtEnd())
            {
                Lox.Error(line, "Unterminated String.");
                return;
            }
            // closing "
            Advance();
            // Trimming Surrounding Qoutes
            String value = source[(start+1)..(current-1)];
            AddToken(TokenType.STRING, value);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
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

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current+1];
        }
        private bool IsAtEnd() { return current >= source.Length; }

    }
}
