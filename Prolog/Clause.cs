namespace Prolog
{
    /// <summary>
    /// Represents a Prolog clause (fact or rule)
    /// Facts have a head and empty body, rules have both head and body
    /// </summary>
    public class Clause
    {
        public Term Head { get; }
        public List<Term> Body { get; }
        public bool IsFact => Body.Count == 0;

        public Clause(Term head, List<Term> body)
        {
            Head = head ?? throw new ArgumentNullException(nameof(head));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public Clause(Term head) : this(head, new List<Term>())
        {
        }

        public override string ToString()
        {
            if (IsFact)
            {
                return $"{Head}.";
            }
            else
            {
                var bodyString = string.Join(", ", Body.Select(term => term.ToString()));
                return $"{Head} :- {bodyString}.";
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Clause other)
                return false;

            if (!Head.Equals(other.Head) || Body.Count != other.Body.Count)
                return false;

            for (int i = 0; i < Body.Count; i++)
            {
                if (!Body[i].Equals(other.Body[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = Head.GetHashCode();
            foreach (var term in Body)
            {
                hash = HashCode.Combine(hash, term.GetHashCode());
            }
            return hash;
        }
    }
}