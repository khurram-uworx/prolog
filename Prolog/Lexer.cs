using System.Text;

namespace Prolog
{
    /// <summary>
    /// Lexical analyzer that converts Prolog source text into a stream of tokens
    /// </summary>
    public class Lexer
    {
        readonly string source;
        int position;
        int line;
        int column;

        public Lexer(string source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            position = 0;
            line = 1;
            column = 1;
        }

        /// <summary>
        /// Tokenizes the entire source text and returns all tokens
        /// </summary>
        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            Token token;
            
            do
            {
                token = NextToken();
                tokens.Add(token);
            } while (token.Type != TokenType.EndOfFile);

            return tokens;
        }

        /// <summary>
        /// Returns the next token from the source text
        /// </summary>
        public Token NextToken()
        {
            if (position >= source.Length)
            {
                return new Token(TokenType.EndOfFile, "", line, column);
            }

            char current = source[position];

            // Skip whitespace but track position
            if (char.IsWhiteSpace(current))
            {
                return ReadWhitespace();
            }

            // Comments start with %
            if (current == '%')
            {
                return ReadComment();
            }

            // Single character tokens
            switch (current)
            {
                case '.':
                    return CreateSingleCharToken(TokenType.Dot);
                case ',':
                    return CreateSingleCharToken(TokenType.Comma);
                case '(':
                    return CreateSingleCharToken(TokenType.LeftParen);
                case ')':
                    return CreateSingleCharToken(TokenType.RightParen);
            }

            // Multi-character operators
            if (current == ':' && Peek() == '-')
            {
                return CreateTwoCharToken(TokenType.Rule, ":-");
            }

            if (current == '?' && Peek() == '-')
            {
                return CreateTwoCharToken(TokenType.Query, "?-");
            }

            if (current == '\\' && Peek() == '=')
            {
                return CreateTwoCharToken(TokenType.NotEqual, "\\=");
            }

            // Atoms and variables
            if (char.IsLetter(current) || current == '_')
            {
                return ReadIdentifier();
            }

            // Invalid character
            return CreateSingleCharToken(TokenType.Invalid);
        }

        Token ReadWhitespace()
        {
            var start = position;
            var startLine = line;
            var startColumn = column;

            while (position < source.Length && char.IsWhiteSpace(source[position]))
            {
                if (source[position] == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
                position++;
            }

            var value = source.Substring(start, position - start);
            return new Token(TokenType.Whitespace, value, startLine, startColumn);
        }

        Token ReadComment()
        {
            var start = position;
            var startLine = line;
            var startColumn = column;

            // Skip the % character
            position++;
            column++;

            // Read until end of line or end of file
            while (position < source.Length && source[position] != '\n')
            {
                position++;
                column++;
            }

            var value = source.Substring(start, position - start);
            return new Token(TokenType.Comment, value, startLine, startColumn);
        }

        Token ReadIdentifier()
        {
            var start = position;
            var startLine = line;
            var startColumn = column;

            // Read the first character
            char first = source[position];
            position++;
            column++;

            // Continue reading alphanumeric characters and underscores
            while (position < source.Length && 
                   (char.IsLetterOrDigit(source[position]) || source[position] == '_'))
            {
                position++;
                column++;
            }

            var value = source.Substring(start, position - start);
            
            // Determine if it's an atom or variable based on first character
            TokenType type = char.IsUpper(first) || first == '_' ? TokenType.Variable : TokenType.Atom;
            
            return new Token(type, value, startLine, startColumn);
        }

        Token CreateSingleCharToken(TokenType type)
        {
            var token = new Token(type, source[position].ToString(), line, column);
            position++;
            column++;
            return token;
        }

        Token CreateTwoCharToken(TokenType type, string value)
        {
            var token = new Token(type, value, line, column);
            position += 2;
            column += 2;
            return token;
        }

        char Peek()
        {
            return position + 1 < source.Length ? source[position + 1] : '\0';
        }
    }
}