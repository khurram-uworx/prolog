using System.Collections.Generic;

namespace Prolog
{
    /// <summary>
    /// Represents a Prolog variable - a placeholder that can be bound to values
    /// Variables start with uppercase letters or underscore
    /// Examples: X, Person, _
    /// </summary>
    public class Variable : Term
    {
        public string Name { get; }

        public Variable(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override bool IsVariable => true;

        public override Term Substitute(Dictionary<string, Term> bindings)
        {
            // If this variable is bound, return the bound term (recursively substituted)
            if (bindings.TryGetValue(Name, out Term? boundTerm))
            {
                return boundTerm.Substitute(bindings);
            }
            
            // If not bound, return this variable unchanged
            return this;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Variable other && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}