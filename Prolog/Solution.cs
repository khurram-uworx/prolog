namespace Prolog
{
    /// <summary>
    /// Represents a solution to a Prolog query with variable bindings
    /// </summary>
    public class Solution
    {
        public Dictionary<string, Term> Bindings { get; }
        public bool IsSuccess { get; }

        Solution(bool isSuccess, Dictionary<string, Term> bindings)
        {
            IsSuccess = isSuccess;
            Bindings = bindings ?? new Dictionary<string, Term>();
        }

        /// <summary>
        /// Creates a successful solution with the given variable bindings
        /// </summary>
        public static Solution Success(Dictionary<string, Term> bindings)
        {
            return new Solution(true, new Dictionary<string, Term>(bindings));
        }

        /// <summary>
        /// Creates a failed solution indicating no match was found
        /// </summary>
        public static Solution Failure()
        {
            return new Solution(false, new Dictionary<string, Term>());
        }

        /// <summary>
        /// Creates a successful solution with no variable bindings
        /// </summary>
        public static Solution SuccessWithNoBindings()
        {
            return new Solution(true, new Dictionary<string, Term>());
        }

        public override string ToString()
        {
            if (!IsSuccess)
                return "No solution";

            if (Bindings.Count == 0)
                return "Success (no bindings)";

            var bindingStrings = Bindings.Select(kvp => $"{kvp.Key} = {kvp.Value}");
            return string.Join(", ", bindingStrings);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Solution other)
                return false;

            if (IsSuccess != other.IsSuccess)
                return false;

            if (Bindings.Count != other.Bindings.Count)
                return false;

            foreach (var kvp in Bindings)
            {
                if (!other.Bindings.TryGetValue(kvp.Key, out Term? otherValue) || 
                    !kvp.Value.Equals(otherValue))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = IsSuccess.GetHashCode();
            foreach (var kvp in Bindings.OrderBy(x => x.Key))
            {
                hash = HashCode.Combine(hash, kvp.Key.GetHashCode(), kvp.Value.GetHashCode());
            }
            return hash;
        }
    }
}