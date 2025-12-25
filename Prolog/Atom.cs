using System.Collections.Generic;

namespace Prolog
{
    /// <summary>
    /// Represents a Prolog atom - a constant symbol starting with lowercase letter
    /// Examples: tom, parent, likes
    /// </summary>
    public class Atom : Term
    {
        public string Name { get; }

        public Atom(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override bool IsVariable => false;

        public override Term Substitute(Dictionary<string, Term> bindings)
        {
            // Atoms are constants, so substitution returns the same atom
            return this;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Atom other && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}