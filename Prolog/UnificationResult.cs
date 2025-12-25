namespace Prolog
{
    /// <summary>
    /// Represents the result of a unification attempt
    /// </summary>
    public class UnificationResult
    {
        public bool Success { get; }
        public Dictionary<string, Term> Bindings { get; }

        UnificationResult(bool success, Dictionary<string, Term> bindings)
        {
            Success = success;
            Bindings = bindings ?? new Dictionary<string, Term>();
        }

        /// <summary>
        /// Creates a successful unification result with the given bindings
        /// </summary>
        public static UnificationResult CreateSuccess(Dictionary<string, Term> bindings)
        {
            return new UnificationResult(true, new Dictionary<string, Term>(bindings));
        }

        /// <summary>
        /// Creates a failed unification result
        /// </summary>
        public static UnificationResult Failure()
        {
            return new UnificationResult(false, new Dictionary<string, Term>());
        }

        /// <summary>
        /// Creates a successful unification result with no bindings
        /// </summary>
        public static UnificationResult SuccessWithNoBindings()
        {
            return new UnificationResult(true, new Dictionary<string, Term>());
        }

        public override string ToString()
        {
            if (!Success)
                return "Unification failed";

            if (Bindings.Count == 0)
                return "Unification succeeded (no bindings)";

            var bindingStrings = Bindings.Select(kvp => $"{kvp.Key} = {kvp.Value}");
            return $"Unification succeeded: {string.Join(", ", bindingStrings)}";
        }
    }
}