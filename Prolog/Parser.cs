namespace Prolog
{
    /// <summary>
    /// Parser that converts Prolog tokens into terms and clauses
    /// </summary>
    public class Parser : IParser
    {
        List<Token>? tokens;
        int position;

        public ParseResult ParseProgram(string source)
        {
            try
            {
                var lexer = new Lexer(source);
                tokens = lexer.Tokenize().Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.Comment).ToList();
                position = 0;

                var clauses = new List<Clause>();

                while (!IsAtEnd())
                {
                    if (Current().Type == TokenType.EndOfFile)
                        break;

                    var clause = ParseClause();
                    if (clause != null)
                    {
                        clauses.Add(clause);
                    }
                }

                return ParseResult.SuccessProgram(clauses);
            }
            catch (ParseException ex)
            {
                return ParseResult.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return ParseResult.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public ParseResult ParseQuery(string source)
        {
            try
            {
                var lexer = new Lexer(source);
                tokens = lexer.Tokenize().Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.Comment).ToList();
                position = 0;

                // Expect query to start with ?-
                if (!Check(TokenType.Query))
                {
                    throw new ParseException("Query must start with '?-'");
                }
                Advance(); // consume ?-

                var query = ParseTerm();
                
                // Handle compound queries (multiple goals separated by commas)
                if (Check(TokenType.Comma))
                {
                    var goals = new List<Term> { query };
                    
                    while (Check(TokenType.Comma))
                    {
                        Advance(); // consume comma
                        goals.Add(ParseTerm());
                    }
                    
                    // Create a compound term representing the conjunction
                    query = new Compound(",", goals);
                }

                // Expect query to end with dot
                if (!Check(TokenType.Dot))
                {
                    throw new ParseException("Query must end with '.'");
                }

                return ParseResult.SuccessQuery(query);
            }
            catch (ParseException ex)
            {
                return ParseResult.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return ParseResult.Failure($"Unexpected error: {ex.Message}");
            }
        }

        Clause? ParseClause()
        {
            var head = ParseTerm();

            if (Check(TokenType.Dot))
            {
                // It's a fact
                Advance(); // consume dot
                return new Clause(head);
            }
            else if (Check(TokenType.Rule))
            {
                // It's a rule
                Advance(); // consume :-
                
                var body = new List<Term>();
                body.Add(ParseTerm());
                
                // Handle multiple goals in body
                while (Check(TokenType.Comma))
                {
                    Advance(); // consume comma
                    body.Add(ParseTerm());
                }
                
                if (!Check(TokenType.Dot))
                {
                    throw new ParseException($"Expected '.' at end of rule at line {Current().Line}");
                }
                Advance(); // consume dot
                
                return new Clause(head, body);
            }
            else
            {
                throw new ParseException($"Expected '.' or ':-' after term at line {Current().Line}");
            }
        }

        Term ParseTerm()
        {
            if (Check(TokenType.Atom))
            {
                var atomToken = Advance();
                
                // Check if it's a compound term
                if (Check(TokenType.LeftParen))
                {
                    Advance(); // consume (
                    
                    var arguments = new List<Term>();
                    
                    if (!Check(TokenType.RightParen))
                    {
                        arguments.Add(ParseTerm());
                        
                        while (Check(TokenType.Comma))
                        {
                            Advance(); // consume comma
                            arguments.Add(ParseTerm());
                        }
                    }
                    
                    if (!Check(TokenType.RightParen))
                    {
                        throw new ParseException($"Expected ')' at line {Current().Line}");
                    }
                    Advance(); // consume )
                    
                    return new Compound(atomToken.Value, arguments);
                }
                else
                {
                    // Simple atom
                    return new Atom(atomToken.Value);
                }
            }
            else if (Check(TokenType.Variable))
            {
                var variableToken = Advance();
                return new Variable(variableToken.Value);
            }
            else
            {
                throw new ParseException($"Expected atom or variable at line {Current().Line}, got {Current().Type}");
            }
        }

        Token Current()
        {
            if (tokens == null || position >= tokens.Count)
                return new Token(TokenType.EndOfFile, "", 0, 0);
            return tokens[position];
        }

        Token Advance()
        {
            if (!IsAtEnd()) position++;
            return Previous();
        }

        Token Previous()
        {
            if (tokens == null || position == 0)
                return new Token(TokenType.EndOfFile, "", 0, 0);
            return tokens[position - 1];
        }

        bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Current().Type == type;
        }

        bool IsAtEnd()
        {
            return tokens == null || position >= tokens.Count || Current().Type == TokenType.EndOfFile;
        }
    }

    /// <summary>
    /// Exception thrown when parsing fails
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}