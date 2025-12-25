namespace Prolog
{
    /// <summary>
    /// Interface for query engine that finds solutions through unification and backtracking
    /// </summary>
    public interface IQueryEngine
    {
        /// <summary>
        /// Solves a query against the knowledge base, returning all solutions
        /// </summary>
        /// <param name="query">The query term to solve</param>
        /// <returns>Enumerable of solutions with variable bindings</returns>
        IEnumerable<Solution> Solve(Term query);
    }
}