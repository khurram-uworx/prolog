namespace Prolog
{
    /// <summary>
    /// Unification engine that implements Prolog unification algorithm
    /// Handles atoms, variables, and compound terms with occurs check
    /// </summary>
    public class UnificationEngine : IUnificationEngine
    {
        readonly bool enableOccursCheck;

        public UnificationEngine(bool enableOccursCheck = true)
        {
            this.enableOccursCheck = enableOccursCheck;
        }

        /// <summary>
        /// Attempts to unify two terms with the given variable bindings
        /// </summary>
        public UnificationResult Unify(Term term1, Term term2, Dictionary<string, Term> bindings)
        {
            if (term1 == null || term2 == null)
                throw new ArgumentNullException("Terms cannot be null");

            if (bindings == null)
                throw new ArgumentNullException(nameof(bindings));

            // Create a copy of bindings to avoid modifying the original
            var newBindings = new Dictionary<string, Term>(bindings);

            // Perform unification
            bool success = UnifyInternal(term1, term2, newBindings);

            return success ? UnificationResult.CreateSuccess(newBindings) : UnificationResult.Failure();
        }

        /// <summary>
        /// Internal unification algorithm
        /// </summary>
        bool UnifyInternal(Term term1, Term term2, Dictionary<string, Term> bindings)
        {
            // Dereference variables (follow binding chains)
            term1 = Dereference(term1, bindings);
            term2 = Dereference(term2, bindings);

            // If terms are identical after dereferencing, they unify
            if (term1.Equals(term2))
                return true;

            // Variable unification
            if (term1 is Variable var1)
            {
                return UnifyVariable(var1, term2, bindings);
            }

            if (term2 is Variable var2)
            {
                return UnifyVariable(var2, term1, bindings);
            }

            // Atom unification - already handled by equality check above
            if (term1 is Atom && term2 is Atom)
            {
                return false; // Different atoms don't unify
            }

            // Compound term unification
            if (term1 is Compound compound1 && term2 is Compound compound2)
            {
                return UnifyCompounds(compound1, compound2, bindings);
            }

            // Different types don't unify
            return false;
        }

        /// <summary>
        /// Unifies a variable with a term
        /// </summary>
        bool UnifyVariable(Variable variable, Term term, Dictionary<string, Term> bindings)
        {
            // Check if variable is already bound
            if (bindings.ContainsKey(variable.Name))
            {
                // Variable is bound, unify with its binding
                return UnifyInternal(bindings[variable.Name], term, bindings);
            }

            // Check if term is a variable that's already bound
            if (term is Variable otherVar && bindings.ContainsKey(otherVar.Name))
            {
                return UnifyInternal(variable, bindings[otherVar.Name], bindings);
            }

            // Occurs check: prevent infinite structures like X = f(X)
            if (enableOccursCheck && OccursCheck(variable, term, bindings))
            {
                return false;
            }

            // Bind the variable to the term
            bindings[variable.Name] = term;
            return true;
        }

        /// <summary>
        /// Unifies two compound terms
        /// </summary>
        bool UnifyCompounds(Compound compound1, Compound compound2, Dictionary<string, Term> bindings)
        {
            // Functors and arities must match
            if (compound1.Functor != compound2.Functor || compound1.Arity != compound2.Arity)
            {
                return false;
            }

            // Unify all arguments recursively
            for (int i = 0; i < compound1.Arguments.Count; i++)
            {
                if (!UnifyInternal(compound1.Arguments[i], compound2.Arguments[i], bindings))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Dereferences a term by following variable bindings
        /// </summary>
        Term Dereference(Term term, Dictionary<string, Term> bindings)
        {
            if (term is Variable variable && bindings.TryGetValue(variable.Name, out Term? boundTerm))
            {
                // Follow the binding chain
                return Dereference(boundTerm, bindings);
            }
            return term;
        }

        /// <summary>
        /// Occurs check: returns true if variable occurs in term
        /// Prevents infinite structures like X = f(X)
        /// </summary>
        bool OccursCheck(Variable variable, Term term, Dictionary<string, Term> bindings)
        {
            term = Dereference(term, bindings);

            if (term is Variable otherVar)
            {
                return variable.Name == otherVar.Name;
            }

            if (term is Compound compound)
            {
                return compound.Arguments.Any(arg => OccursCheck(variable, arg, bindings));
            }

            return false;
        }

        /// <summary>
        /// Convenience method to unify two terms with empty bindings
        /// </summary>
        public UnificationResult Unify(Term term1, Term term2)
        {
            return Unify(term1, term2, new Dictionary<string, Term>());
        }

        /// <summary>
        /// Checks if two terms can be unified (without returning bindings)
        /// </summary>
        public bool CanUnify(Term term1, Term term2)
        {
            return Unify(term1, term2).Success;
        }

        /// <summary>
        /// Checks if two terms can be unified with given bindings
        /// </summary>
        public bool CanUnify(Term term1, Term term2, Dictionary<string, Term> bindings)
        {
            return Unify(term1, term2, bindings).Success;
        }
    }
}