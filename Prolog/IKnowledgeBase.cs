namespace Prolog
{
    /// <summary>
    /// Interface for knowledge base that stores and retrieves Prolog clauses
    /// </summary>
    public interface IKnowledgeBase
    {
        /// <summary>
        /// Gets the total number of clauses in the knowledge base
        /// </summary>
        int ClauseCount { get; }

        /// <summary>
        /// Adds a clause to the knowledge base
        /// </summary>
        /// <param name="clause">The clause to add</param>
        void AddClause(Clause clause);

        /// <summary>
        /// Retrieves all clauses that could potentially match the given goal
        /// </summary>
        /// <param name="goal">The goal term to match against</param>
        /// <returns>Enumerable of matching clauses</returns>
        IEnumerable<Clause> GetMatchingClauses(Term goal);

        /// <summary>
        /// Clears all clauses from the knowledge base
        /// </summary>
        void Clear();
    }
}