namespace Prolog
{
    /// <summary>
    /// Represents the type of a lexical token in Prolog source code
    /// </summary>
    public enum TokenType
    {
        // Literals
        Atom,           // tom, parent, likes
        Variable,       // X, Person, _
        
        // Operators and punctuation
        Dot,            // .
        Comma,          // ,
        LeftParen,      // (
        RightParen,     // )
        Rule,           // :-
        
        // Special
        Query,          // ?-
        Comment,        // % comment text
        Whitespace,     // spaces, tabs, newlines
        EndOfFile,      // end of input
        
        // Error
        Invalid         // unrecognized character
    }

    /// <summary>
    /// Represents a lexical token with its type, value, and position information
    /// </summary>
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int Line { get; }
        public int Column { get; }

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"{Type}('{Value}') at {Line}:{Column}";
        }

        public override bool Equals(object? obj)
        {
            return obj is Token other && 
                   Type == other.Type && 
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }
}