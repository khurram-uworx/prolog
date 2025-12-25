namespace Prolog
{
    /// <summary>
    /// Represents the result of parsing Prolog source code
    /// </summary>
    public class ParseResult
    {
        public bool Success { get; }
        public List<Clause> Clauses { get; }
        public Term? Query { get; }
        public string ErrorMessage { get; }

        ParseResult(bool success, List<Clause> clauses, Term? query, string errorMessage)
        {
            Success = success;
            Clauses = clauses ?? new List<Clause>();
            Query = query;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public static ParseResult SuccessProgram(List<Clause> clauses)
        {
            return new ParseResult(true, clauses, null, string.Empty);
        }

        public static ParseResult SuccessQuery(Term query)
        {
            return new ParseResult(true, new List<Clause>(), query, string.Empty);
        }

        public static ParseResult Failure(string errorMessage)
        {
            return new ParseResult(false, new List<Clause>(), null, errorMessage);
        }
    }
}