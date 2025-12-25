namespace Prolog
{
    /// <summary>
    /// Interface for unification engine that implements pattern matching
    /// </summary>
    public interface IUnificationEngine
    {
        /// <summary>
        /// Attempts to unify two terms with the given variable bindings
        /// </summary>
        /// <param name="term1">First term to unify</param>
        /// <param name="term2">Second term to unify</param>
        /// <param name="bindings">Existing variable bindings</param>
        /// <returns>Unification result with success status and updated bindings</returns>
        UnificationResult Unify(Term term1, Term term2, Dictionary<string, Term> bindings);
    }
}