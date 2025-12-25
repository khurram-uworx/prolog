namespace Prolog
{
    /// <summary>
    /// Knowledge base that stores and retrieves Prolog clauses (facts and rules)
    /// Maintains insertion order for deterministic behavior
    /// </summary>
    public class KnowledgeBase : IKnowledgeBase
    {
        readonly List<Clause> clauses;
        readonly Dictionary<string, List<Clause>> indexByFunctor;

        public int ClauseCount => clauses.Count;

        public KnowledgeBase()
        {
            clauses = new List<Clause>();
            indexByFunctor = new Dictionary<string, List<Clause>>();
        }

        /// <summary>
        /// Adds a clause to the knowledge base
        /// Maintains insertion order and updates functor index
        /// </summary>
        public void AddClause(Clause clause)
        {
            if (clause == null)
                throw new ArgumentNullException(nameof(clause));

            clauses.Add(clause);

            // Index by functor for efficient retrieval
            var functor = GetFunctor(clause.Head);
            if (!indexByFunctor.ContainsKey(functor))
            {
                indexByFunctor[functor] = new List<Clause>();
            }
            indexByFunctor[functor].Add(clause);
        }

        /// <summary>
        /// Retrieves all clauses that could potentially match the given goal
        /// Returns clauses with the same functor as the goal
        /// </summary>
        public IEnumerable<Clause> GetMatchingClauses(Term goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            var functor = GetFunctor(goal);
            
            if (indexByFunctor.TryGetValue(functor, out List<Clause>? matchingClauses))
            {
                // Return in insertion order
                return matchingClauses;
            }

            return Enumerable.Empty<Clause>();
        }

        /// <summary>
        /// Clears all clauses from the knowledge base
        /// </summary>
        public void Clear()
        {
            clauses.Clear();
            indexByFunctor.Clear();
        }

        /// <summary>
        /// Returns all clauses in insertion order
        /// </summary>
        public IEnumerable<Clause> GetAllClauses()
        {
            return clauses;
        }

        /// <summary>
        /// Returns all clauses with the specified functor
        /// </summary>
        public IEnumerable<Clause> GetClausesByFunctor(string functor)
        {
            if (indexByFunctor.TryGetValue(functor, out List<Clause>? matchingClauses))
            {
                return matchingClauses;
            }
            return Enumerable.Empty<Clause>();
        }

        /// <summary>
        /// Gets the functor/arity signature for indexing purposes
        /// </summary>
        string GetFunctor(Term term)
        {
            if (term is Atom atom)
            {
                return $"{atom.Name}/0";
            }
            else if (term is Compound compound)
            {
                return $"{compound.Functor}/{compound.Arity}";
            }
            else if (term is Variable variable)
            {
                // Variables don't have a specific functor, but we need to handle them
                // In practice, variables in goals will be unified against concrete terms
                return $"_var_{variable.Name}";
            }
            else
            {
                throw new ArgumentException($"Unknown term type: {term.GetType()}");
            }
        }

        /// <summary>
        /// Returns statistics about the knowledge base
        /// </summary>
        public KnowledgeBaseStats GetStats()
        {
            var factCount = clauses.Count(c => c.IsFact);
            var ruleCount = clauses.Count(c => !c.IsFact);
            var functorCount = indexByFunctor.Keys.Count;

            return new KnowledgeBaseStats
            {
                TotalClauses = clauses.Count,
                FactCount = factCount,
                RuleCount = ruleCount,
                UniqueFunctors = functorCount
            };
        }
    }

    /// <summary>
    /// Statistics about the knowledge base contents
    /// </summary>
    public class KnowledgeBaseStats
    {
        public int TotalClauses { get; set; }
        public int FactCount { get; set; }
        public int RuleCount { get; set; }
        public int UniqueFunctors { get; set; }

        public override string ToString()
        {
            return $"Total: {TotalClauses}, Facts: {FactCount}, Rules: {RuleCount}, Functors: {UniqueFunctors}";
        }
    }
}