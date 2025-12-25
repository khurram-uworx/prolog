namespace Prolog
{
    /// <summary>
    /// Query engine that implements Prolog query resolution using unification and backtracking
    /// Handles both simple and compound queries with multiple goals
    /// </summary>
    public class QueryEngine : IQueryEngine
    {
        readonly IKnowledgeBase knowledgeBase;
        readonly IUnificationEngine unificationEngine;

        public QueryEngine(IKnowledgeBase knowledgeBase, IUnificationEngine unificationEngine)
        {
            this.knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
            this.unificationEngine = unificationEngine ?? throw new ArgumentNullException(nameof(unificationEngine));
        }

        /// <summary>
        /// Solves a query against the knowledge base, returning all solutions
        /// </summary>
        public IEnumerable<Solution> Solve(Term query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            // Handle compound queries (multiple goals)
            if (query is Compound compound && compound.Functor == "," && compound.Arity == 2)
            {
                // This is a compound query like "parent(X, Y), parent(Y, Z)"
                return SolveCompoundQuery(compound, new Dictionary<string, Term>());
            }

            // Handle simple queries
            return SolveGoal(query, new Dictionary<string, Term>());
        }

        /// <summary>
        /// Solves a compound query with multiple goals using backtracking
        /// </summary>
        IEnumerable<Solution> SolveCompoundQuery(Compound compoundQuery, Dictionary<string, Term> bindings)
        {
            if (compoundQuery.Functor != "," || compoundQuery.Arity != 2)
                throw new ArgumentException("Expected compound query with comma operator");

            var firstGoal = compoundQuery.Arguments[0];
            var secondGoal = compoundQuery.Arguments[1];

            // Solve the first goal
            foreach (var firstSolution in SolveGoal(firstGoal, bindings))
            {
                if (!firstSolution.IsSuccess)
                    continue;

                // Apply bindings from first goal to second goal
                var substitutedSecondGoal = secondGoal.Substitute(firstSolution.Bindings);

                // Handle nested compound queries recursively
                if (substitutedSecondGoal is Compound nestedCompound && 
                    nestedCompound.Functor == "," && nestedCompound.Arity == 2)
                {
                    foreach (var nestedSolution in SolveCompoundQuery(nestedCompound, firstSolution.Bindings))
                    {
                        if (nestedSolution.IsSuccess)
                        {
                            var resolvedBindings = ResolveBindings(nestedSolution.Bindings);
                            yield return Solution.Success(resolvedBindings);
                        }
                    }
                }
                else
                {
                    // Solve the second goal with bindings from the first
                    foreach (var secondSolution in SolveGoal(substitutedSecondGoal, firstSolution.Bindings))
                    {
                        if (secondSolution.IsSuccess)
                        {
                            var resolvedBindings = ResolveBindings(secondSolution.Bindings);
                            yield return Solution.Success(resolvedBindings);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Solves a single goal against the knowledge base
        /// </summary>
        IEnumerable<Solution> SolveGoal(Term goal, Dictionary<string, Term> bindings)
        {
            // Handle built-in predicates first
            if (goal is Compound compound)
            {
                switch (compound.Functor)
                {
                    case "\\=" when compound.Arity == 2:
                        // Not unifiable predicate
                        foreach (var solution in SolveNotEqual(compound.Arguments[0], compound.Arguments[1], bindings))
                        {
                            yield return solution;
                        }
                        yield break;
                }
            }

            // Get all clauses that could potentially match this goal
            var matchingClauses = knowledgeBase.GetMatchingClauses(goal);

            bool foundSolution = false;

            foreach (var clause in matchingClauses)
            {
                // Rename variables in the clause to avoid conflicts
                var renamedClause = RenameVariables(clause);

                // Try to unify the goal with the clause head
                var unificationResult = unificationEngine.Unify(goal, renamedClause.Head, bindings);

                if (unificationResult.Success)
                {
                    if (renamedClause.IsFact)
                    {
                        // This is a fact - we have a solution
                        foundSolution = true;
                        var resolvedBindings = ResolveBindings(unificationResult.Bindings);
                        yield return Solution.Success(resolvedBindings);
                    }
                    else
                    {
                        // This is a rule - we need to solve the body goals
                        foreach (var bodySolution in SolveRuleBody(renamedClause.Body, unificationResult.Bindings))
                        {
                            if (bodySolution.IsSuccess)
                            {
                                foundSolution = true;
                                var resolvedBindings = ResolveBindings(bodySolution.Bindings);
                                yield return Solution.Success(resolvedBindings);
                            }
                        }
                    }
                }
            }

            // If no solutions were found, this is not necessarily a failure
            // The caller will determine if this constitutes a failure
            if (!foundSolution)
            {
                // Don't yield a failure solution here - let the caller handle it
                // This allows compound queries to backtrack properly
            }
        }

        /// <summary>
        /// Handles the \= (not unifiable) built-in predicate
        /// </summary>
        IEnumerable<Solution> SolveNotEqual(Term term1, Term term2, Dictionary<string, Term> bindings)
        {
            // Substitute variables with their bindings
            var substitutedTerm1 = term1.Substitute(bindings);
            var substitutedTerm2 = term2.Substitute(bindings);

            // Check if the terms can be unified
            var canUnify = unificationEngine.CanUnify(substitutedTerm1, substitutedTerm2, bindings);

            if (!canUnify)
            {
                // Terms cannot be unified, so \= succeeds
                yield return Solution.Success(bindings);
            }
            // If terms can be unified, \= fails (no solutions yielded)
        }

        /// <summary>
        /// Solves the body of a rule (list of goals) using backtracking
        /// </summary>
        IEnumerable<Solution> SolveRuleBody(List<Term> bodyGoals, Dictionary<string, Term> bindings)
        {
            if (bodyGoals.Count == 0)
            {
                // Empty body means the rule succeeds with current bindings
                var resolvedBindings = ResolveBindings(bindings);
                yield return Solution.Success(resolvedBindings);
                yield break;
            }

            if (bodyGoals.Count == 1)
            {
                // Single goal in body
                var substitutedGoal = bodyGoals[0].Substitute(bindings);
                foreach (var solution in SolveGoal(substitutedGoal, bindings))
                {
                    if (solution.IsSuccess)
                        yield return solution;
                }
                yield break;
            }

            // Multiple goals in body - solve them sequentially with backtracking
            var firstGoal = bodyGoals[0];
            var remainingGoals = bodyGoals.Skip(1).ToList();

            var substitutedFirstGoal = firstGoal.Substitute(bindings);

            foreach (var firstSolution in SolveGoal(substitutedFirstGoal, bindings))
            {
                if (!firstSolution.IsSuccess)
                    continue;

                // Solve remaining goals with bindings from first goal
                foreach (var remainingSolution in SolveRuleBody(remainingGoals, firstSolution.Bindings))
                {
                    if (remainingSolution.IsSuccess)
                        yield return remainingSolution;
                }
            }
        }

        /// <summary>
        /// Renames all variables in a clause to avoid conflicts with query variables
        /// </summary>
        Clause RenameVariables(Clause clause)
        {
            var variableMap = new Dictionary<string, string>();
            var counter = 0;

            // Collect all variables in the clause
            var allVariables = new HashSet<string>();
            CollectVariables(clause.Head, allVariables);
            foreach (var bodyTerm in clause.Body)
            {
                CollectVariables(bodyTerm, allVariables);
            }

            // Create renamed versions
            foreach (var varName in allVariables)
            {
                variableMap[varName] = $"_{varName}_{counter++}";
            }

            // Apply renaming
            var renamedHead = RenameVariablesInTerm(clause.Head, variableMap);
            var renamedBody = clause.Body.Select(term => RenameVariablesInTerm(term, variableMap)).ToList();

            return new Clause(renamedHead, renamedBody);
        }

        /// <summary>
        /// Collects all variable names in a term
        /// </summary>
        void CollectVariables(Term term, HashSet<string> variables)
        {
            if (term is Variable variable)
            {
                variables.Add(variable.Name);
            }
            else if (term is Compound compound)
            {
                foreach (var arg in compound.Arguments)
                {
                    CollectVariables(arg, variables);
                }
            }
        }

        /// <summary>
        /// Renames variables in a term according to the variable map
        /// </summary>
        Term RenameVariablesInTerm(Term term, Dictionary<string, string> variableMap)
        {
            if (term is Variable variable && variableMap.TryGetValue(variable.Name, out string? newName))
            {
                return new Variable(newName);
            }
            else if (term is Compound compound)
            {
                var renamedArgs = compound.Arguments.Select(arg => RenameVariablesInTerm(arg, variableMap)).ToList();
                return new Compound(compound.Functor, renamedArgs);
            }
            else
            {
                // Atoms don't need renaming
                return term;
            }
        }

        /// <summary>
        /// Filters bindings to only include variables that appear in the goal
        /// </summary>
        Dictionary<string, Term> FilterBindingsToGoalVariables(Term goal, Dictionary<string, Term> bindings)
        {
            var goalVariables = new HashSet<string>();
            CollectVariables(goal, goalVariables);

            var filteredBindings = new Dictionary<string, Term>();
            foreach (var kvp in bindings)
            {
                if (goalVariables.Contains(kvp.Key))
                {
                    filteredBindings[kvp.Key] = kvp.Value;
                }
            }

            return filteredBindings;
        }

        /// <summary>
        /// Resolves all variable binding chains to their final values
        /// </summary>
        Dictionary<string, Term> ResolveBindings(Dictionary<string, Term> bindings)
        {
            var resolvedBindings = new Dictionary<string, Term>();
            
            foreach (var kvp in bindings)
            {
                var resolvedValue = kvp.Value.Substitute(bindings);
                resolvedBindings[kvp.Key] = resolvedValue;
            }
            
            return resolvedBindings;
        }
    }
}