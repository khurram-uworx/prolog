namespace Prolog
{
    /// <summary>
    /// Pretty printer that formats internal structures back into valid Prolog syntax
    /// </summary>
    public class PrettyPrinter
    {
        /// <summary>
        /// Formats a term into valid Prolog syntax
        /// </summary>
        public string FormatTerm(Term term)
        {
            return term.ToString();
        }

        /// <summary>
        /// Formats a clause into valid Prolog syntax
        /// </summary>
        public string FormatClause(Clause clause)
        {
            return clause.ToString();
        }

        /// <summary>
        /// Formats a program (list of clauses) into valid Prolog syntax
        /// </summary>
        public string FormatProgram(List<Clause> clauses)
        {
            return string.Join("\n", clauses.Select(clause => clause.ToString()));
        }

        /// <summary>
        /// Formats a query into valid Prolog syntax
        /// </summary>
        public string FormatQuery(Term query)
        {
            // Handle compound queries (conjunctions)
            if (query is Compound compound && compound.Functor == ",")
            {
                var goals = string.Join(", ", compound.Arguments.Select(arg => arg.ToString()));
                return $"?- {goals}.";
            }
            else
            {
                return $"?- {query}.";
            }
        }

        /// <summary>
        /// Formats a program with nice indentation and spacing
        /// </summary>
        public string FormatProgramPretty(List<Clause> clauses)
        {
            var result = new List<string>();
            
            foreach (var clause in clauses)
            {
                if (clause.IsFact)
                {
                    result.Add(clause.ToString());
                }
                else
                {
                    // Format rules with indented body
                    var bodyString = string.Join(",\n    ", clause.Body.Select(term => term.ToString()));
                    result.Add($"{clause.Head} :-\n    {bodyString}.");
                }
            }
            
            return string.Join("\n\n", result);
        }

        /// <summary>
        /// Formats a term with proper parenthesization to avoid ambiguity
        /// </summary>
        public string FormatTermWithParens(Term term)
        {
            if (term is Compound compound && compound.Arity > 0)
            {
                // Always use parentheses for compound terms to be explicit
                var argStrings = compound.Arguments.Select(arg => FormatTermWithParens(arg));
                return $"{compound.Functor}({string.Join(", ", argStrings)})";
            }
            else
            {
                return term.ToString();
            }
        }
    }
}