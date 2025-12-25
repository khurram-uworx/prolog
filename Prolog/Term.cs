using System.Collections.Generic;

namespace Prolog
{
    /// <summary>
    /// Base class for all Prolog terms (atoms, variables, compound terms)
    /// </summary>
    public abstract class Term
    {
        /// <summary>
        /// Returns true if this term is a variable
        /// </summary>
        public abstract bool IsVariable { get; }

        /// <summary>
        /// Substitutes variables in this term according to the provided bindings
        /// </summary>
        /// <param name="bindings">Variable name to term mappings</param>
        /// <returns>New term with variables substituted</returns>
        public abstract Term Substitute(Dictionary<string, Term> bindings);

        /// <summary>
        /// Returns the Prolog string representation of this term
        /// </summary>
        public abstract override string ToString();

        /// <summary>
        /// Determines equality based on term structure and content
        /// </summary>
        public abstract override bool Equals(object? obj);

        /// <summary>
        /// Returns hash code based on term structure and content
        /// </summary>
        public abstract override int GetHashCode();
    }
}